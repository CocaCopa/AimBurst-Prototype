using System;
using AimBurst.Core.Contracts;
using UnityEngine;

namespace AimBurst.PrefabRegistry {
    public static class PrefabAPI {
        private static PrefabCatalog catalog;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            catalog = Resources.Load<PrefabCatalog>("PrefabCatalog");
            if (!catalog) throw new Exception("Missing Resources/PrefabCatalog.asset");
        }

        public static GameObject InstantiateShooter(CubeColor id, Transform parent = null) => CreateCubes(CubeType.Shooter, id, parent);
        public static GameObject InstantiateTarget(CubeColor id, Transform parent = null) => CreateCubes(CubeType.Cube, id, parent);
        public static GameObject InstantiateBullet(Transform parent = null) => Instantiate(catalog.GetBullet(), parent);

        private static GameObject CreateCubes(CubeType type, CubeColor id, Transform parent) {
            var prefab = type == CubeType.Shooter ? catalog.GetShooter(id) : catalog.GetCube(id);
            return Instantiate(prefab, parent);
        }

        private static GameObject Instantiate(GameObject prefab, Transform parent) {
            var obj = UnityEngine.Object.Instantiate(prefab, parent);
            string name = obj.name.Replace("(Clone)", " - Clone");
            obj.name = name;
            return obj;
        }

        private enum CubeType { Shooter, Cube };
    }
}
