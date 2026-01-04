using AimBurst.UI.Unity.EndScreen;
using CocaCopa.EditorUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AimBurst.UI.EditorTools.EndScreen {
    [CustomEditor(typeof(EndScreenUI))]
    public sealed class EndScreenUIEditor : Editor {
        private SerializedProperty confetti;
        private SerializedProperty victoryBanner;
        private SerializedProperty nextLevelBtnObj;
        private SerializedProperty backgroundImg;

        private void OnEnable() {
            confetti = serializedObject.FindProperty(nameof(confetti));
            victoryBanner = serializedObject.FindProperty(nameof(victoryBanner));
            nextLevelBtnObj = serializedObject.FindProperty(nameof(nextLevelBtnObj));
            backgroundImg = serializedObject.FindProperty(nameof(backgroundImg));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorCommon.DisplayScriptReference(target);

            bool enabled = IsScreenEnabled();

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(enabled);
            if (GUILayout.Button("Enable Screen")) {
                SetScreenEnabled(true);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!enabled);
            if (GUILayout.Button("Disable Screen")) {
                SetScreenEnabled(false);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            DrawPropertiesExcluding(serializedObject, "m_Script");

            serializedObject.ApplyModifiedProperties();
        }

        private bool IsScreenEnabled() {
            var bg = backgroundImg.objectReferenceValue as Image;
            if (bg == null) return false;
            return bg.color.a > 0.001f;
        }

        private void SetScreenEnabled(bool enabled) {
            ToggleConfetti(enabled);
            SetBackgroundAlpha(enabled ? 1f : 0f);
            SetScale(victoryBanner, enabled);
            SetScale(nextLevelBtnObj, enabled);
        }

        // ===============================
        // Specific responsibilities
        // ===============================

        private void ToggleConfetti(bool enabled) {
            if (confetti == null) return;

            ToggleGameObject(confetti.FindPropertyRelative("particlesObj"), enabled, "Toggle Confetti Particles");
            ToggleGameObject(confetti.FindPropertyRelative("textureObj"), enabled, "Toggle Confetti Texture");
        }

        private void SetBackgroundAlpha(float alpha) {
            var img = backgroundImg.objectReferenceValue as Image;
            if (img == null) return;

            Undo.RecordObject(img, "Change Background Alpha");

            var c = img.color;
            c.a = alpha;
            img.color = c;

            EditorUtility.SetDirty(img);
        }

        private void SetScale(SerializedProperty prop, bool enabled) {
            if (prop == null) return;

            Transform t = null;

            if (prop.objectReferenceValue is Component c)
                t = c.transform;
            else if (prop.objectReferenceValue is GameObject go)
                t = go.transform;

            if (t == null) return;

            Undo.RecordObject(t, "Change UI Scale");
            t.localScale = enabled ? Vector3.one : Vector3.zero;
            EditorUtility.SetDirty(t);
        }

        private static void ToggleGameObject(SerializedProperty prop, bool enabled, string undoName) {
            if (prop == null) return;

            var go = prop.objectReferenceValue as GameObject;
            if (go == null) return;

            Undo.RecordObject(go, undoName);
            go.SetActive(enabled);
            EditorUtility.SetDirty(go);
        }
    }
}
