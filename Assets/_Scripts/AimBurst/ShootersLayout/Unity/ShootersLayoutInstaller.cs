using System.Threading;
using AimBurst.ShootersLayout.API;
using AimBurst.ShootersLayout.Runtime;
using AimBurst.ShootersLayout.Runtime.Abstractions;
using AimBurst.ShootersLayout.Unity.Pathing;
using AimBurst.ShootersLayout.Unity.SLots;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity {
    [DisallowMultipleComponent]
    internal sealed class ShootersLayoutInstaller : MonoBehaviour {
        [Tooltip("Time in seconds at which a click will register to the flow.")]
        [SerializeField] private float clickGate;
        [Tooltip("Delay before a shooter is allowed to leave their slot after their ammo is fully depleted.")]
        [SerializeField] private float releaseDelay;
        [Space(10)]
        [SerializeField] private Transform layoutHolder;
        [SerializeField] private Transform slotsHolder;
        [SerializeField] private ShootersBezier shootersBezier;
        [SerializeField] private SlotsAnimator slotsAnimator;

        private CancellationTokenSource cancellationSource;

        private void Awake() {
            cancellationSource = new CancellationTokenSource();
            ShooterLaneController.ClearInstances();

            var totalSlots = slotsHolder.childCount;
            var totalSpawners = layoutHolder.childCount;

            var slotsManager = new SlotsManager(slotsAnimator, totalSlots, (int)(releaseDelay * 1000), cancellationSource.Token);
            slotsManager.Init();
            SlotStateAPI.Wire(slotsManager);
            var flow = new ShootersLayoutFlow((int)(clickGate * 1000f), slotsManager);
            flow.Init();

            for (int i = 0; i < totalSpawners; i++) {
                var child = layoutHolder.GetChild(i);

                var spawner = child.GetComponent<ShootersLayoutSpawner>();
                spawner.Init(shootersBezier);

                var controller = new ShooterLaneController(spawner.Index, spawner.SpawnedShooters<IShooterLane>(), spawner.Spacing);
                controller.Init();
                LaneStateAPI.Wire(controller);

                var adapter = child.GetComponent<ShooterLaneAdapter>();
                adapter.Wire(flow, controller);
            }
        }

        private void OnDestroy() {
            cancellationSource.Cancel();
            cancellationSource.Dispose();
        }
    }
}
