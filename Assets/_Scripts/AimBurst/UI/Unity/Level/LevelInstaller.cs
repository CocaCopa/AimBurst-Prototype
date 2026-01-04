using AimBurst.UI.API.Level;
using UnityEngine;

namespace AimBurst.UI.Unity.Level {
    internal sealed class LevelInstaller : MonoBehaviour {
        [SerializeField] private LevelUI levelProgressionUI;

        private void Awake() {
            LevelAPI.Wire(levelProgressionUI);
        }
    }
}
