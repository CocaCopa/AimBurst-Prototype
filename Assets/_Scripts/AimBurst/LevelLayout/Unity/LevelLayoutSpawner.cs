using System;
using System.Collections.Generic;
using AimBurst.Core.Contracts;
using AimBurst.LevelLayout.Contracts;
using AimBurst.LevelLayout.Unity.Columns;
using AimBurst.PrefabRegistry;
using UnityEngine;

namespace AimBurst.LevelLayout.Unity {
    public sealed class LevelLayoutSpawner : MonoBehaviour {
        [Header("References")]
        [SerializeField] private LevelLayoutEnvironment environment;
        [SerializeField] private Transform columnsHolder;

        [Header("Cube Placement")]
        [SerializeField] private Vector3 refSpawnPos;
        [SerializeField] private float cubeSpacing;

        [Header("Column Settings")]
        [SerializeField] private int maxAvailableCubes;

        [Space(10)]
        [SerializeField] private bool isHardLevel;
        [SerializeField] private LayoutConfig[] layoutConfig = new LayoutConfig[10];

        private readonly Dictionary<int, Dictionary<int, Queue<ICube>>> byColumn = new();
        private readonly Dictionary<int, Dictionary<CubeColor, Queue<ICube>>> byColor = new();

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
        internal Dictionary<int, Dictionary<int, Queue<ICube>>> ByColumn => byColumn;

        /// <summary>
        /// Column-based lookup of positions grouped by cube color.
        /// <list type="bullet">
        ///   <item>
        ///     <description><b>Key</b>: Column index</description>
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
        internal Dictionary<int, Dictionary<CubeColor, Queue<ICube>>> ByColor => byColor;

#if UNITY_EDITOR
        private void OnValidate() {
            if (layoutConfig == null) { return; }

            for (int i = 0; i < layoutConfig.Length; i++) {
                var config = layoutConfig[i];
                config.name = $"Column {i + 1}";

                if (config.sets != null) {
                    for (int j = 0; j < config.sets.Length; j++) {
                        var set = config.sets[j];
                        set.name = $"Set {j + 1}";
                        config.sets[j] = set;
                    }
                }

                layoutConfig[i] = config;
            }
        }
#endif

        internal void Init() {
            SpawnColumns(layoutConfig);
            environment.Init(isHardLevel);
        }

        private void SpawnColumns(LayoutConfig[] configs) {
            if (configs == null || configs.Length == 0) { return; }
            if (columnsHolder == null) {
                Debug.LogError($"[{nameof(LevelLayoutSpawner)}] '{nameof(columnsHolder)}' is not assigned.");
                return;
            }

            byColumn.Clear();
            byColor.Clear();

            for (int columnIndex = 0; columnIndex < configs.Length; columnIndex++) {
                var column = CreateColumn(columnIndex);
                SpawnColumnSets(column, configs[columnIndex], columnIndex);
            }
        }

        private Transform CreateColumn(int columnIndex) {
            var column = new GameObject($"Column {columnIndex + 1}").transform;
            column.SetParent(columnsHolder);
            column.localPosition = refSpawnPos + cubeSpacing * columnIndex * Vector3.left;
            column.gameObject.AddComponent<GridColumn>();
            return column;
        }

        private void SpawnColumnSets(Transform column, LayoutConfig config, int columnIndex) {
            if (config.sets == null || config.sets.Length == 0) { return; }

            // IMPORTANT: We pad missing floors with null so every floor queue stays aligned by "depth index".
            // That requires spawning each set up to the maximum floors used by any set in this column.
            int maxFloorsInColumn = 0;
            for (int i = 0; i < config.sets.Length; i++) {
                if (config.sets[i].floors > maxFloorsInColumn) {
                    maxFloorsInColumn = config.sets[i].floors;
                }
            }

            float zOffset = 0f;
            for (int i = 0; i < config.sets.Length; i++) {
                var set = config.sets[i];
                SpawnSet(column, set, zOffset, columnIndex, maxFloorsInColumn);
                zOffset += set.count * cubeSpacing;
            }
        }

        private void SpawnSet(Transform parent, SetConfig set, float zOffset, int columnIndex, int maxFloorsInColumn) {
            if (set.count <= 0) { return; }
            if (maxFloorsInColumn <= 0) { return; }

            for (int i = 0; i < set.count; i++) {
                float z = -(zOffset + i * cubeSpacing);

                for (int floorIndex = 0; floorIndex < maxFloorsInColumn; floorIndex++) {
                    if (floorIndex < set.floors) {
                        float height = floorIndex * cubeSpacing;

                        var cube = SpawnCube(parent, set.color, new Vector3(0f, height, z));
                        AddByFloor(floorIndex, columnIndex, cube);
                        AddByColor(floorIndex, set.color, cube);
                    }
                    else {
                        // Pad earlier positions with null so the floor's queue represents "empty slots" at that depth.
                        AddByFloor(floorIndex, columnIndex, null);
                        // Don't put null into byColor (it's a color index; null has no color).
                    }
                }
            }
        }

        private void AddByFloor(int floorIndex, int columnIndex, ICube cube) {
            if (!byColumn.TryGetValue(floorIndex, out var columnsMap)) {
                columnsMap = new Dictionary<int, Queue<ICube>>();
                byColumn.Add(floorIndex, columnsMap);
            }

            if (!columnsMap.TryGetValue(columnIndex, out var cubes)) {
                cubes = new Queue<ICube>();
                columnsMap.Add(columnIndex, cubes);
            }

            cubes.Enqueue(cube); // FIFO (including null padding)
        }

        private void AddByColor(int floorIndex, CubeColor color, ICube cube) {
            // NOTE: We intentionally ignore nulls here.
            if (cube == null) { return; }

            if (!byColor.TryGetValue(floorIndex, out var colorMap)) {
                colorMap = new Dictionary<CubeColor, Queue<ICube>>();
                byColor.Add(floorIndex, colorMap);
            }

            if (!colorMap.TryGetValue(color, out var cubes)) {
                cubes = new Queue<ICube>();
                colorMap.Add(color, cubes);
            }

            cubes.Enqueue(cube); // FIFO
        }

        private static ICube SpawnCube(Transform parent, CubeColor color, Vector3 localPos) {
            var cube = PrefabAPI.InstantiateTarget(color, parent);
            cube.transform.localPosition = localPos;

            if (cube != null && cube.TryGetComponent<Cube>(out var cubeComp)) {
                return cubeComp;
            }

            throw new NullReferenceException($"[{nameof(LevelLayoutSpawner)}] {nameof(Cube)}");
        }

        [Serializable]
        private struct LayoutConfig {
            [HideInInspector] public string name;
            public SetConfig[] sets;
        }

        [Serializable]
        private struct SetConfig {
            [HideInInspector] public string name;
            public CubeColor color;
            public int count;
            public int floors;
        }
    }
}
