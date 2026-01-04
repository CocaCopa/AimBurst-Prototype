using System;

namespace AimBurst.SceneControls.API {
    public interface ISceneController {
        event Action<int> OnNextLevelLoaded;
        void LoadNextLevel();
        void RestartCurrentLevel();
        int? NextIndex { get; }
        int CurrentIndex { get; }
    }
}
