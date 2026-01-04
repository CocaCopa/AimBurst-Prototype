using System;
using System.IO;
using AimBurst.SceneControls.API;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AimBurst.SceneControls.Unity {
    public class SceneController : MonoBehaviour, ISceneController {
        private const char SceneSplitChar = '_';
        private const string LevelPrefix = "Level";

        public event Action<int> OnNextLevelLoaded;

        /// <summary>
        /// Current level NUMBER (the # in "Level_#"). If you're not in a level scene, returns 0.
        /// </summary>
        public int CurrentIndex => GetCurrentLevelNumberOrZero();

        /// <summary>
        /// Next level NUMBER if "Level_{CurrentIndex+1}" exists in Build Settings; otherwise null.
        /// </summary>
        public int? NextIndex {
            get {
                int next = CurrentIndex + 1;
                string nextName = $"{LevelPrefix}_{next}";
                return SceneExistsInBuild(nextName) ? next : (int?)null;
            }
        }

        private void Awake() {
            SceneManager.sceneLoaded += SceneManager_SceneLoaded;
        }

        private void OnDestroy() {
            SceneManager.sceneLoaded -= SceneManager_SceneLoaded;
        }

        private void SceneManager_SceneLoaded(Scene scene, LoadSceneMode mode) {
            int levelNumber = TryParseLevelNumber(scene.name, out int parsed) ? parsed : 0;
            OnNextLevelLoaded?.Invoke(levelNumber);
        }

        public void LoadNextLevel() {
            int current = CurrentIndex;
            int next = current + 1;
            string nextName = $"{LevelPrefix}_{next}";

            if (!SceneExistsInBuild(nextName)) {
                throw new Exception($"<color=white>[{nameof(SceneController)}]</color> Scene '{nextName}' does not exist");
            }

            SceneManager.LoadScene(nextName);
        }

        public void RestartCurrentLevel() {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        private static int GetCurrentLevelNumberOrZero() {
            string sceneName = SceneManager.GetActiveScene().name;
            return TryParseLevelNumber(sceneName, out int levelNumber) ? levelNumber : 0;
        }

        private static bool TryParseLevelNumber(string sceneName, out int levelNumber) {
            levelNumber = 0;

            if (string.IsNullOrEmpty(sceneName)) return false;

            // Must start with "Level_"
            if (!sceneName.StartsWith(LevelPrefix, StringComparison.Ordinal)) return false;

            int underscoreIndex = sceneName.IndexOf(SceneSplitChar);
            if (underscoreIndex < 0) return false;

            string[] split = sceneName.Split(SceneSplitChar);
            if (split.Length < 2) return false;

            // split[0] should be "Level"
            if (!string.Equals(split[0], LevelPrefix, StringComparison.Ordinal)) return false;

            return int.TryParse(split[1], out levelNumber);
        }

        private static bool SceneExistsInBuild(string sceneName) {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string name = Path.GetFileNameWithoutExtension(path);

                if (string.Equals(name, sceneName, StringComparison.Ordinal)) {
                    return true;
                }
            }
            return false;
        }
    }
}
