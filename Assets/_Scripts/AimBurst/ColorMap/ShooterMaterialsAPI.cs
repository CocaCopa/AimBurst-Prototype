using AimBurst.Core.Contracts;
using UnityEngine;

namespace AimBurst.ColorMap {
    public static class ShooterMaterialsAPI {
        private static ShooterMaterialRegistry registry;
        private static bool initialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInit() {
            if (initialized) return;

            registry = Resources.Load<ShooterMaterialRegistry>("MaterialMap");

            if (registry == null)
                throw new System.Exception(
                    $"<color=white>[{nameof(ShooterMaterialsAPI)}]</color> MaterialMap not found in Resources"
                );

            registry.Init();
            initialized = true;
        }

        public static Material Get(CubeColor color) {
            if (!initialized)
                throw new System.Exception($"<color=white>[{nameof(ShooterMaterialsAPI)}]</color> Accessed before initialization");

            return registry.Get(color);
        }
    }
}
