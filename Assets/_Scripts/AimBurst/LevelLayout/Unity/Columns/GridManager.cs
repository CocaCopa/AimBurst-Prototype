using System;
using System.Collections.Generic;
using AimBurst.LevelLayout.Runtime.Abstractions;
using UnityEngine;

namespace AimBurst.LevelLayout.Unity.Columns {
    internal sealed class GridManager : MonoBehaviour, IGridManager {
        [Header("Column Setup")]
        [SerializeField] private float moveDelay;
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField, Min(0f)] private float moveDuration;
        [SerializeField, Min(0f)] private float moveOffset;

        private readonly Dictionary<int, GridColumn> columns = new Dictionary<int, GridColumn>();

        internal void Init() {
            for (int i = 0; i < transform.childCount; i++) {
                var child = transform.GetChild(i);
                if (child.TryGetComponent<GridColumn>(out var comp)) {
                    comp.Init(moveDelay, moveCurve, moveDuration, moveOffset);
                    columns[i] = comp;
                }
                else { throw new NullReferenceException($"<color=white>[{nameof(GridManager)}]</color> {nameof(GridColumn)}"); }
            }
        }

        public void MoveDown(int columnIndex) {
            if (columns.TryGetValue(columnIndex, out var column)) {
                column.MoveDown();
            }
            else { Debug.LogError($"<color=white>[{nameof(GridManager)}]</color> Couldn't find requested column"); }
        }
    }
}
