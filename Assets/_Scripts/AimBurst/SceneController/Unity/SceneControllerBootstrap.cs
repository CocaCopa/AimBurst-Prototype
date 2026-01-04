using AimBurst.SceneControls.API;
using UnityEngine;

namespace AimBurst.SceneControls.Unity {
    internal static class SceneControllerBootstrap {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap() {
            SceneController controller = Object.FindFirstObjectByType<SceneController>();

            if (controller == null) {
                var go = new GameObject("SceneController");
                controller = go.AddComponent<SceneController>();
                Object.DontDestroyOnLoad(go);
            }

            SceneControllerAPI.Wire(controller);
        }
    }
}
