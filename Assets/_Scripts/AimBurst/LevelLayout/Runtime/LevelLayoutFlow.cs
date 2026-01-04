using System;
using System.Collections.Generic;
using AimBurst.Core.Contracts;
using AimBurst.LevelLayout.Contracts;
using AimBurst.LevelLayout.Runtime.Abstractions;
using CocaCopa.Logger.API;
using CocaCopa.Primitives;

namespace AimBurst.LevelLayout.Runtime {
    internal sealed class LevelLayoutFlow : ILevelLayout, ILevelState {

        /// <summary>
        /// Column-based lookup of positions grouped by floor.
        /// <list type="bullet">
        ///   <item>
        ///     <description><b>Key</b>: Floor index</description>
        ///   </item>
        ///   <item>
        ///     <description><b>Value</b>: Dictionary where:</description>
        ///     <list type="bullet">
        ///       <item><description><b>Key</b>: Column index within the floor</description></item>
        ///       <item><description><b>Value</b>: Queue of instances implementing <see cref="ICube"/></description></item>
        ///     </list>
        ///   </item>
        /// </list>
        /// </summary>
        private readonly Dictionary<int, Dictionary<int, Queue<ICube>>> byColumnQ = new Dictionary<int, Dictionary<int, Queue<ICube>>>();
        private readonly Dictionary<int, Dictionary<int, List<ICube>>> byColumn = new Dictionary<int, Dictionary<int, List<ICube>>>();

        /// <summary>
        /// Column-based lookup of positions grouped by cube color.
        /// <list type="bullet">
        ///   <item>
        ///     <description><b>Key</b>: Floor index</description>
        ///   </item>
        ///   <item>
        ///     <description><b>Value</b>: Dictionary where:</description>
        ///     <list type="bullet">
        ///       <item><description><b>Key</b>: Cube color</description></item>
        ///       <item><description><b>Value</b>: Queue of instances implementing <see cref="ICube"/></description></item>
        ///     </list>
        ///   </item>
        /// </list>
        /// </summary>
        private readonly Dictionary<int, Dictionary<CubeColor, Queue<ICube>>> byColor = new Dictionary<int, Dictionary<CubeColor, Queue<ICube>>>();
        private readonly Dictionary<ICube, int> pendingMoveByIntendedTarget = new Dictionary<ICube, int>();
        private readonly IGridManager gridManager;
        private readonly object closestCubeLock = new object();

        public int TotalTargetsCount { get; private set; }
        public int CurrentTargetsCount { get; private set; }

        internal LevelLayoutFlow(LayoutLookups lookups, IGridManager gridManager) {
            byColumnQ = lookups.byColumnQueue;
            byColor = lookups.byColorQueue;
            this.gridManager = gridManager;
        }

        /// <summary>
        /// Initializes derived lookup structures.
        /// <para>
        /// Must be called after all cubes have been registered in
        /// <see cref="byColumn"/> and <see cref="byColor"/>.
        /// </para>
        /// </summary>
        internal void Init() {
            foreach (var floorPair in byColumnQ) {
                int floor = floorPair.Key;
                var columnMapQ = floorPair.Value;

                var columnMapL = new Dictionary<int, List<ICube>>();

                foreach (var columnPair in columnMapQ) {
                    int column = columnPair.Key;
                    Queue<ICube> queue = columnPair.Value;

                    columnMapL[column] = new List<ICube>(queue);
                }

                byColumn[floor] = columnMapL;
            }

            TotalTargetsCount = 0;
            foreach (var floorMap in byColumn) {
                foreach (var columnMap in floorMap.Value) {
                    var column = columnMap.Value;
                    TotalTargetsCount += column.Count;
                }
            }
            CurrentTargetsCount = TotalTargetsCount;
        }

        public bool TryGetClosestCube(C_Vector3 yourPos, CubeColor targetColor, out ICube result) {
            lock (closestCubeLock) {
                result = null;
                float minDist = float.MaxValue;
                int foundFloor = -1;
                int foundColumn = -1;

                // Search from top floor to bottom.
                for (int floor = byColumn.Count - 1; floor >= 0; floor--) {
                    if (!byColumn.TryGetValue(floor, out var columnMap)) {
                        throw new Exception("Floors have to be contiguous");
                    }

                    bool floorHasTarget = false;

                    foreach (var column in columnMap) {
                        int columnIndex = column.Key;
                        var cubes = column.Value;

                        if (cubes?.Count == 0) { continue; }

                        var frontCube = cubes[0];
                        if (frontCube?.Color != targetColor) { continue; }

                        if (yourPos.z - frontCube.Position.z > 7) { continue; }
                        float dist = C_Vector3.Distance(frontCube.Position, yourPos);
                        if (dist < minDist) {
                            minDist = dist;
                            foundFloor = floor;
                            foundColumn = columnIndex;
                            floorHasTarget = true;
                            result = frontCube;
                        }
                    }

                    if (floorHasTarget) { break; }
                }

                if (result == null) { return false; }

                byColumn[foundFloor][foundColumn][0] = null;
                CurrentTargetsCount--;

                if (foundFloor == 0) {
                    RemoveAllFloorEntriesFromColumn(foundColumn);
                    pendingMoveByIntendedTarget[result] = foundColumn;
                }
                return true;
            }
        }

        private void RemoveAllFloorEntriesFromColumn(int columnIndex) {
            foreach (var floor in byColumn.Values) {
                if (!floor.TryGetValue(columnIndex, out var list)) {
                    continue;
                }
                if (list.Count == 0) { continue; }
                list.RemoveAt(0);
            }
        }

        public void ReportHit(ICube hitTarget, ICube intendedTarget, C_Vector3 yourMoveDir) {
            lock (closestCubeLock) {
                if (intendedTarget == null || hitTarget == null) {
                    throw new NullReferenceException($"<color=white>[{nameof(LevelLayoutFlow)}] {nameof(ICube)}");
                }

                if (hitTarget == intendedTarget) {
                    hitTarget.Kill();
                    if (pendingMoveByIntendedTarget.TryGetValue(intendedTarget, out int columnToMove)) {
                        pendingMoveByIntendedTarget.Remove(intendedTarget);
                        gridManager.MoveDown(columnToMove);
                    }
                }
                else {
                    hitTarget.DodgeBullet(yourMoveDir);
                }
            }
        }
    }
}
