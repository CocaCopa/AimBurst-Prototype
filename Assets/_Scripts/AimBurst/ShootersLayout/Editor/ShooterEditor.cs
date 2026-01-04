using AimBurst.ShootersLayout.Unity.Actor;
using CocaCopa.EditorUtils;
using UnityEditor;

namespace AimBurst.ShootersLayout.EditorTools {
    [CustomEditor(typeof(Shooter))]
    internal sealed class ShooterEditor : Editor {
        private SerializedProperty combatFriend;

        private void OnEnable() {
            combatFriend = serializedObject.FindProperty(nameof(combatFriend));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorCommon.DisplayScriptReference(target, 10f);
            DrawCombatFriend();
            DrawPropertiesExcluding(serializedObject, ExcludeProperties());
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCombatFriend() {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(combatFriend);
            EditorGUI.EndDisabledGroup();
        }

        private string[] ExcludeProperties() {
            return new string[2] {
                "m_Script",
                nameof(combatFriend)
            };
        }
    }
}
