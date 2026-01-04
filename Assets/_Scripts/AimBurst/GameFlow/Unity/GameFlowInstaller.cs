using System.Threading;
using AimBurst.GameFlow.Runtime;
using UnityEngine;

namespace AimBurst.GameFlow.Unity {
    internal sealed class GameFlowInstaller : MonoBehaviour {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private GameFlowCoordinator flow;

        private void Awake() {
            flow = new GameFlowCoordinator(cts.Token);
        }

        private void Start() {
            flow.Init();
        }

        private void OnDestroy() {
            cts.Cancel();
            cts.Dispose();
        }
    }
}
