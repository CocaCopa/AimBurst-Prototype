using System;
using System.Collections.Generic;
using System.Linq;
using AimBurst.Core.Contracts;
using UnityEngine;

namespace AimBurst.PrefabRegistry {
    [CreateAssetMenu(fileName = "PrefabCatalog", menuName = "Game/Prefab Catalog")]
    internal class PrefabCatalog : ScriptableObject {
        [SerializeField] private ShooterEntry[] shooterEntries;
        [SerializeField] private CubeEntry[] cubeEntries;
        [SerializeField] private BulletEntry bulletEntry;

        private Dictionary<CubeColor, GameObject> shootersMap;
        private Dictionary<CubeColor, GameObject> cubesMap;

        private void OnValidate() {
            for (int i = 0; i < shooterEntries.Length; i++) {
                var c = shooterEntries[i];
                c.name = $"Shooter {i}";
                shooterEntries[i] = c;
            }

            for (int i = 0; i < cubeEntries.Length; i++) {
                var c = cubeEntries[i];
                c.name = $"Cube {i}";
                cubeEntries[i] = c;
            }
        }

        public GameObject GetShooter(CubeColor id) {
            shootersMap ??= shooterEntries.ToDictionary(e => e.id, e => e.prefab);
            return shootersMap[id];
        }

        public GameObject GetCube(CubeColor id) {
            cubesMap ??= cubeEntries.ToDictionary(e => e.id, e => e.prefab);
            return cubesMap[id];
        }

        public GameObject GetBullet() {
            return bulletEntry.bullet;
        }

        [Serializable]
        public struct ShooterEntry {
            [HideInInspector] public string name;
            public CubeColor id;
            public GameObject prefab;
        }

        [Serializable]
        public struct CubeEntry {
            [HideInInspector] public string name;
            public CubeColor id;
            public GameObject prefab;
        }

        [Serializable]
        public struct BulletEntry {
            public GameObject bullet;
        }
    }
}
