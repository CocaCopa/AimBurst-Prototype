using AimBurst.LevelLayout.API;
using AimBurst.LevelLayout.Runtime;
using AimBurst.LevelLayout.Runtime.Abstractions;
using AimBurst.LevelLayout.Unity.Columns;
using UnityEngine;

namespace AimBurst.LevelLayout.Unity {
    internal sealed class LevelLayoutInstaller : MonoBehaviour {
        private void Awake() {
            var spawner = GetComponentInChildrenSafe<LevelLayoutSpawner>();
            var gridManager = GetComponentInChildrenSafe<GridManager>();
            if (spawner == null) { return; }

            spawner.Init();
            gridManager.Init();
            var lookups = new LayoutLookups(spawner.ByColumn, spawner.ByColor);
            var flow = new LevelLayoutFlow(lookups, gridManager);
            flow.Init();
            LevelLayoutAPI.Wire(flow);
            LevelLayoutStateAPI.Wire(flow);
        }

        private T GetComponentInChildrenSafe<T>() where T : MonoBehaviour {
            var found = transform.GetComponentInChildren<T>();
            if (found == null) {
                Debug.LogError($"<color=white>[{nameof(LevelLayoutInstaller)}]</color> Missing component in children.");
                return null;
            }
            return found;
        }
    }
}