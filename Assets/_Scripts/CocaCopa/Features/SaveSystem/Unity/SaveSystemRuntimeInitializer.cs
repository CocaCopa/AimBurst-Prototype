using UnityEngine;

namespace CocaCopa.SaveSystem.Unity {
    internal static class SaveSystemRuntimeInitializer {
        private static string ResourcePath => "SaveSystemConfig";
        private static SaveSystemConfig config;
        private static bool initialized;

        private static SaveSystemConfig Config {
            get {
                if (config != null) return config;
                config = Resources.Load<SaveSystemConfig>(ResourcePath);
                return config;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void OnAfterAssembliesLoaded() {
            TryInitialize(SaveSystemInitPhase.AfterAssembliesLoaded);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void OnBeforeSplashScreen() {
            TryInitialize(SaveSystemInitPhase.BeforeSplashScreen);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration() {
            TryInitialize(SaveSystemInitPhase.SubsystemRegistration);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad() {
            TryInitialize(SaveSystemInitPhase.BeforeSceneLoad);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad() {
            TryInitialize(SaveSystemInitPhase.AfterSceneLoad);
        }

        private static void TryInitialize(SaveSystemInitPhase phase) {
            if (initialized) return;

            var cfg = Config;
            if (cfg == null) return;

            if (cfg.InitPhase != phase) return;

            cfg.Initialize();
            initialized = true;
        }
    }
}
