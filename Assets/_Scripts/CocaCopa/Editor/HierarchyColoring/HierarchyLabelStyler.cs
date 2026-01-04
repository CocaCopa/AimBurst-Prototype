using System;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.EditorUtils.HierarchyColoring {
    [InitializeOnLoad]
    public static class HierarchyLabelStyler {
        static HierarchyLabelStyler() {
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
        }

        private static void OnGUI(int instanceID, Rect selectionRect) {
            GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            string name = obj.name;

            var config = HierarchyColorSettingsWindow.GetConfig();
            if (name.StartsWith(config.targetString)) {
                bool isSelected = Array.IndexOf(Selection.instanceIDs, instanceID) >= 0;

                if (isSelected) {
                    EditorGUI.DrawRect(selectionRect, new Color32(44, 93, 135, 255));
                }
                else {
                    EditorGUI.DrawRect(selectionRect, config.backgroundColor);
                }

                GUIStyle style = new GUIStyle(EditorStyles.label) {
                    fontStyle = config.fontStyle,
                    normal = new GUIStyleState {
                        textColor = config.fontColor
                    }
                };

                Rect labelRect = new Rect(selectionRect);
                labelRect.x += 14;
                EditorGUI.LabelField(labelRect, " " + name, style);
                return;
            }
        }
    }
}
