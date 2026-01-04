using System;
using System.IO;
using CocaCopa.SaveSystem.Unity;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.SaveSystem.EditorTools {
    [CustomEditor(typeof(SaveSystemConfig))]
    internal class SaveSystemConfigEditor : Editor {
        private SerializedProperty initPhase;

        private SerializedProperty saveDestination;
        private SerializedProperty customPath;

        private SerializedProperty prettyPrintJson;

        private SerializedProperty useEncryption;
        private SerializedProperty passphrase;
        private SerializedProperty saltHex;

        private void OnEnable() {
            FindProperties();
        }

        private void FindProperties() {
            initPhase = serializedObject.FindProperty(nameof(initPhase));

            saveDestination = serializedObject.FindProperty(nameof(saveDestination));
            customPath = serializedObject.FindProperty(nameof(customPath));

            prettyPrintJson = serializedObject.FindProperty(nameof(prettyPrintJson));

            useEncryption = serializedObject.FindProperty(nameof(useEncryption));
            passphrase = serializedObject.FindProperty(nameof(passphrase));
            saltHex = serializedObject.FindProperty(nameof(saltHex));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            GUI.enabled = !Application.isPlaying;
            DrawInitialization();
            DrawStorage();
            DrawSerialization();
            DrawEncryption();
            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInitialization() {
            EditorGUILayout.LabelField("Initialization", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(initPhase);
            EditorGUILayout.Space(10);
        }

        private void DrawStorage() {
            EditorGUILayout.LabelField("Storage", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(saveDestination);
            if (saveDestination.enumValueIndex == 0) {
                EditorGUILayout.PropertyField(customPath, new GUIContent("Custom Path"));
            }
            else if (saveDestination.enumValueIndex != 1) {
                EditorGUILayout.PropertyField(customPath, new GUIContent("Sub Folder"));
            }

            GUI.enabled = true;
            if (GUILayout.Button("Open Folder Path")) {
                try {
                    string root = (target as SaveSystemConfig).GetRootFolder();
                    if (string.IsNullOrWhiteSpace(root)) {
                        Debug.LogWarning("[SaveSystemConfigEditor] Root folder is null or empty.");
                        return;
                    }
                    if (!Directory.Exists(root)) {
                        Directory.CreateDirectory(root);
                        Debug.Log($"[SaveSystemConfigEditor] Folder did not exist. Created new folder at:{root}");
                    }
                    EditorUtility.RevealInFinder(root + "/");
                }
                catch (System.Exception ex) {
                    Debug.LogError($"[SaveSystemConfigEditor] Failed to open save folder: {ex}");
                }
            }
            GUI.enabled = !Application.isPlaying;
            EditorGUILayout.Space(10);
        }

        private void DrawSerialization() {
            EditorGUILayout.LabelField("Serialization", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(useEncryption.boolValue);
            {
                EditorGUILayout.PropertyField(prettyPrintJson);
                if (useEncryption.boolValue) prettyPrintJson.boolValue = false;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(10);
        }

        private void DrawEncryption() {
            EditorGUILayout.LabelField("Encryption", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useEncryption);
            EditorGUI.BeginDisabledGroup(!useEncryption.boolValue);
            {
                EditorGUILayout.PropertyField(passphrase);
                EditorGUILayout.PropertyField(saltHex);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(10);
        }
    }
}
