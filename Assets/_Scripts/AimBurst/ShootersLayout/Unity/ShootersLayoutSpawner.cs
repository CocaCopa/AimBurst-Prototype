using System;
using System.Collections.Generic;
using System.Linq;
using AimBurst.Core.Contracts;
using AimBurst.PrefabRegistry;
using AimBurst.ShootersLayout.Unity.Abstractions;
using AimBurst.ShootersLayout.Unity.Actor;
using AimBurst.ShootersLayout.Unity.Pathing;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity {
    [RequireComponent(typeof(ShooterLaneAdapter))]
    internal sealed class ShootersLayoutSpawner : MonoBehaviour {
        [SerializeField] private int index;
        [Space(10)]
        [Tooltip("Determine the position offset of the shooter when spawned")]
        [SerializeField] private Vector3 spawnOffset;
        [Tooltip("Determine the distance between each shooter")]
        [SerializeField] private float shootersDistance;
        [Space(10)]
        [SerializeField] private List<ShootersConfig> shootersConfig = new List<ShootersConfig>();

        private const string HolderName = "ShootersHolder";
        private readonly List<IShooterInit> shooters = new List<IShooterInit>();
        private Transform shootersHolder;
        private bool initFailed = false;
        private ShootersBezier shootersBezier;

        public List<T> SpawnedShooters<T>() where T : class {
            var result = shooters.OfType<T>().ToList();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (result.Count == 0)
                Debug.LogWarning($"[{nameof(ShootersLayoutSpawner)}] No shooters implement {typeof(T).Name}.");
#endif
            return result;
        }

        public float Spacing => shootersDistance;
        public int Index => index;

        internal void Init(ShootersBezier shootersBezier) {
            this.shootersBezier = shootersBezier;
            FindHolderObject();
            CreateShooters();
        }

        private void FindHolderObject() {
            shootersHolder = transform.Find(HolderName);
            if (shootersHolder == null) {
                initFailed = true;
                Debug.LogError($"<color=white>[{nameof(ShootersLayoutSpawner)}]</color> Couldn't find '{HolderName}' child object");
            }
        }

        private void CreateShooters() {
            if (initFailed) { return; }
            float prevDist = 0f;

            for (int i = 0; i < shootersConfig.Count; i++) {
                var shooterColor = shootersConfig[i].color;
                var shootersCount = shootersConfig[i].count;
                for (int j = 0; j < shootersCount; j++) {
                    var shooter = PrefabAPI.InstantiateShooter(shooterColor, shootersHolder);
                    var dist = prevDist + (shootersDistance * j);
                    var currPos = shooter.transform.position;
                    currPos -= shooter.transform.forward * dist;
                    shooter.transform.position = currPos + spawnOffset;
                    if (shooter.TryGetComponent<IShooterInit>(out var comp)) {
                        comp.WireShootersBezierRef(shootersBezier);
                        comp.SetOwnerIndex(index);
                        comp.SetAmmo(shootersConfig[i].ammoCount);
                        comp.SetHidden(shootersConfig[i].hide);
                        shooters.Add(comp);
                    }
                    else throw new NullReferenceException($"Missing '{nameof(Shooter)}' component from shooter object");
                }
                prevDist += shootersConfig[i].count * shootersDistance;
            }
        }

        [Serializable]
        private struct ShootersConfig {
            public CubeColor color;
            public int count;
            public int ammoCount;
            public bool hide;
        }
    }
}
