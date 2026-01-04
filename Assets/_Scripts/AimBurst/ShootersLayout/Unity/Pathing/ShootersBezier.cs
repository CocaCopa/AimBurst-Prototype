using System.Collections.Generic;
using CocaCopa.Unity.Components;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity.Pathing {
    internal sealed class ShootersBezier : MonoBehaviour {
        [SerializeField] private Transform slotPathsHolder;
        [SerializeField] private Transform moveOutPathsHolder;

        private List<List<BezierPath>> slotPathComponents = new List<List<BezierPath>>();
        private List<List<BezierPath>> moveOutPathComponents = new List<List<BezierPath>>();

        private void Awake() {
            slotPathComponents = FetchPathComponents(slotPathsHolder);
            moveOutPathComponents = FetchPathComponents(moveOutPathsHolder);
            InitializePaths();
        }

        private List<List<BezierPath>> FetchPathComponents(Transform fromHolder) {
            var components = new List<List<BezierPath>>();
            for (int i = 0; i < fromHolder.childCount; i++) {
                components.Add(new List<BezierPath>());
                for (int j = 0; j < fromHolder.GetChild(i).childCount; j++) {
                    var child = fromHolder.GetChild(i).GetChild(j);
                    if (child.TryGetComponent<BezierPath>(out var path)) {
                        components[i].Add(path);
                    }
                    else { Debug.LogError($"<color=white>[{nameof(ShootersBezier)}] Couldn't get component: {nameof(BezierPath)}"); }
                }
            }
            return components;
        }

        private void InitializePaths() {
            for (int i = 0; i < slotPathComponents.Count; i++) {
                for (int j = 0; j < slotPathComponents[i].Count; j++) {
                    var path = slotPathComponents[i][j];
                    path.Stop();
                    path.Reset();
                    path.SetTarget(null);
                }
            }
        }

        /// <summary>
        /// Retrieves the <see cref="BezierPath"/> instance from the cached hierarchy
        /// at the specified position index and slot index.
        /// </summary>
        /// <param name="fromPos">
        /// Index of the shooter position.
        /// </param>
        /// <param name="toSlot">
        /// Index of the slot position
        /// the <see cref="BezierPath"/> component.
        /// </param>
        internal BezierPath GetSlotPathInstance(int fromPos, int toSlot) {
            if (fromPos < 0 || fromPos >= slotPathComponents.Count) {
                Debug.LogError(
                    $"<color=white>[{nameof(ShootersBezier)}]</color> Invalid shooter position index '{fromPos}'. " +
                    $"Valid range: 0-{slotPathComponents.Count - 1}."
                );
                return null;
            }

            var position = slotPathComponents[fromPos];

            if (toSlot < 0 || toSlot >= position.Count) {
                Debug.LogError(
                    $"<color=white>[{nameof(ShootersBezier)}]</color> Invalid slot index '{toSlot}' for position '{fromPos}'. " +
                    $"Valid range: 0-{position.Count - 1}."
                );
                return null;
            }

            return position[toSlot];
        }

        /// <summary>
        /// Retrieves the <see cref="BezierPath"/> instance from the cached hierarchy
        /// at the specified position index and slot index.
        /// </summary>
        /// <param name="fromPos">
        /// Index of the shooter position.
        /// </param>
        /// <param name="curved">
        /// True for a curved path, False for a straight path.
        /// the <see cref="BezierPath"/> component.
        /// </param>
        internal BezierPath GetMoveOutPathInstance(int fromPos, bool curved) {
            if (fromPos < 0 || fromPos >= moveOutPathComponents.Count) {
                Debug.LogError(
                    $"<color=white>[{nameof(ShootersBezier)}]</color> Invalid shooter position index '{fromPos}'. " +
                    $"Valid range: 0-{moveOutPathComponents.Count - 1}."
                );
                return null;
            }

            return curved && moveOutPathComponents[fromPos][0].name.Contains("Straight")
            ? moveOutPathComponents[fromPos][1]
            : moveOutPathComponents[fromPos][0];
        }
    }
}
