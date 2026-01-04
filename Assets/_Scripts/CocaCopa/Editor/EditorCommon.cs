using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.EditorUtils {
    public static class EditorCommon {
        /// <summary>
        /// Displays the script reference field in the inspector for a MonoBehaviour or ScriptableObject.
        /// </summary>
        /// <param name="targetObject">The target MonoBehaviour or ScriptableObject.</param>
        public static void DisplayScriptReference(Object targetObject, float space = 0f) {
            if (targetObject is MonoBehaviour || targetObject is ScriptableObject) {
                SerializedObject m_serializedObject = new SerializedObject(targetObject);
                SerializedProperty scriptProperty = m_serializedObject.FindProperty("m_Script");

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(scriptProperty, true);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space(space);
            }
            else {
                Debug.LogError("Target must be a MonoBehaviour or ScriptableObject.");
            }
        }

        public static void CreateFoldout(ref bool expand, string header, Dictionary<SerializedProperty, GUIContent> properties, bool toggleOnLabelClick = true, GUIStyle headerStyle = default) {
            if (headerStyle == default) headerStyle = EditorStyles.foldoutHeader;
            expand = EditorGUILayout.Foldout(expand, header, toggleOnLabelClick, headerStyle);
            if (expand) {
                EditorGUI.indentLevel++;
                foreach (var kvp in properties) {
                    EditorGUILayout.PropertyField(kvp.Key, kvp.Value);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}
