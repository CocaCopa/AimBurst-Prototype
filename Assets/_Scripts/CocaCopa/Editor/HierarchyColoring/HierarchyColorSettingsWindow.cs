using UnityEditor;
using UnityEngine;
namespace CocaCopa.EditorUtils.HierarchyColoring {
    public class HierarchyColorSettingsWindow : EditorWindow {
        private static HierarchyColorConfig Config {
            get; set;
        }
        private static readonly string ConfigPath = "Assets/Editor/Data/HierarchyColorSettings.asset";

        [MenuItem("Tools/Hierarchy Color")]
        public static void ShowWindow() {
            GetWindow<HierarchyColorSettingsWindow>("Hierarchy Color");
        }

        private void OnEnable() {
            LoadOrCreateConfig();
        }

        private void OnGUI() {
            if (Config == null) {
                EditorGUILayout.HelpBox("Config asset missing!", MessageType.Error);
                return;
            }

            EditorGUI.BeginChangeCheck();

            Config.targetString = EditorGUILayout.TextField("Target String", Config.targetString);
            Config.fontColor = EditorGUILayout.ColorField("Font Color", Config.fontColor);
            Config.backgroundColor = EditorGUILayout.ColorField("Background Color", Config.backgroundColor);
            Config.fontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style", Config.fontStyle);

            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(Config);
                AssetDatabase.SaveAssets();
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        private static void LoadOrCreateConfig() {
            Config = AssetDatabase.LoadAssetAtPath<HierarchyColorConfig>(ConfigPath);

            if (Config == null) {
                Config = ScriptableObject.CreateInstance<HierarchyColorConfig>();
                AssetDatabase.CreateAsset(Config, ConfigPath);
                AssetDatabase.SaveAssets();
                Debug.Log("Created new HierarchyColorConfig at " + ConfigPath);
            }
        }

        public static HierarchyColorConfig GetConfig() {
            if (Config == null)
                LoadOrCreateConfig();
            return Config;
        }
    }
}
