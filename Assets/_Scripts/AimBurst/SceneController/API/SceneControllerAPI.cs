using System;

namespace AimBurst.SceneControls.API {
    public static class SceneControllerAPI {
        private static ISceneController sceneController;
        private static ISceneController Controller => sceneController ?? throw new NullReferenceException($"<color=white>[{nameof(SceneControllerAPI)}]</color> {nameof(ISceneController)}");

        internal static void Wire(ISceneController impl) => sceneController = impl;

        public static event Action<int> OnNextLevelLoaded {
            add { Controller.OnNextLevelLoaded += value; }
            remove { Controller.OnNextLevelLoaded -= value; }
        }
        public static void LoadNextLevel() => Controller.LoadNextLevel();
        public static void RestartCurrentLevel() => Controller.RestartCurrentLevel();
        public static int CurrentIndex => Controller.CurrentIndex;
        public static int? NextIndex => Controller.NextIndex;
    }
}
