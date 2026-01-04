using CocaCopa.Modal.Unity;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.Modal.EditorTools {
    [CustomEditor(typeof(ModalInstaller))]
    public class ModalInstallerEditor : Editor {
        private CanvasGroup cg;

        private void OnEnable() {
            if (!EditorApplication.isPlaying) {
                FindCanvasGroup();
                cg.alpha = 1f;
            }
        }

        private void FindCanvasGroup() {
            var installer = target as ModalInstaller;
            if (installer.transform.GetChild(0).TryGetComponent<CanvasGroup>(out var cg)) {
                this.cg = cg;
            }
            else {
                this.cg = installer.transform.GetChild(0).gameObject.AddComponent<CanvasGroup>();
                Debug.Log($"Could not find 'CanvasGroup' component on {installer.gameObject.name}. Added automatically");
            }
        }

        public override void OnInspectorGUI() {
            if (!EditorApplication.isPlaying) {
                ShowUtilityButtons();
            }
            base.OnInspectorGUI();
        }

        private void ShowUtilityButtons() {
            EditorGUILayout.Space(10);
            GUI.enabled = cg.alpha < 1f;
            if (GUILayout.Button("Show Modal")) {
                SetVisible(true);
            }
            GUI.enabled = cg.alpha > 0f;
            if (GUILayout.Button("Hide Modal")) {
                SetVisible(false);
            }
            GUI.enabled = true;
            EditorGUILayout.Space(10);
        }

        private void SetVisible(bool visible) {
            var alpha = visible ? 1f : 0f;
            cg.alpha = alpha;
        }
    }
}
