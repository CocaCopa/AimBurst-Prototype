using AimBurst.UI.API.EndScreen;
using AimBurst.UI.Unity.EndScreen;
using UnityEngine;

namespace AimBurst.UI.Contracts.EndScreen {
    public sealed class EndScreenInstaller : MonoBehaviour {
        [SerializeField] private EndScreenUI endScreenUI;

        private void Awake() {
            EndScreenAPI.Wire(endScreenUI);
        }
    }
}
