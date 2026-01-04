using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.EditorUtils {
    public static class EditorTags {
        /// <summary>
        /// Searches for a tag containing the specified string.
        /// </summary>
        /// <param name="targetString">The string to search for in tags.</param>
        /// <param name="debugLog">If true, logs the result to the console.</param>
        /// <returns>The first matching tag or an empty string if none is found.</returns>
        public static string SearchForTagContaining(string targetString, bool debugLog = false) {
            string matchingTag = "";

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++) {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Contains(targetString)) {
                    matchingTag = t.stringValue;
                }
            }

            if (debugLog) {
                if (matchingTag == string.Empty) {
                    Debug.LogWarning("No tags found containing: " + targetString);
                }
                else {
                    Debug.Log("Tags found containing: " + targetString);
                }
            }
            return matchingTag;
        }

        /// <summary>
        /// Searches for all tags containing the specified string.
        /// </summary>
        /// <param name="targetString">The string to search for in tags.</param>
        /// <param name="debugLog">If true, logs the result to the console.</param>
        /// <returns>A list of all matching tags.</returns>
        public static List<string> SearchForTagsContaining(string targetString, bool debugLog = false) {
            List<string> matchingTags = new List<string>();

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++) {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Contains(targetString)) {
                    matchingTags.Add(t.stringValue);
                }
            }

            if (debugLog) {
                if (matchingTags.Count == 0) {
                    Debug.LogWarning("No tags found containing: " + targetString);
                }
                else {
                    Debug.Log("Tags found containing: " + targetString);
                }
            }
            return matchingTags;
        }

        /// <summary>
        /// Creates a new tag in the project settings if it does not already exist.
        /// </summary>
        /// <param name="tag">The name of the tag to create.</param>
        /// <param name="debugLog">If true, logs the result to the console.</param>
        public static void CreateNewTag(string tag, bool debugLog = false) {
            if (string.IsNullOrEmpty(tag)) {
                Debug.LogWarning("Tag name cannot be empty.");
                return;
            }

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool tagExists = false;
            for (int i = 0; i < tagsProp.arraySize; i++) {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tag)) {
                    tagExists = true;
                    break;
                }
            }

            if (!tagExists) {
                tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
                newTagProp.stringValue = tag;
            }

            if (debugLog) {
                if (!tagExists) {
                    Debug.Log("Tag added: " + tag);
                }
                else {
                    Debug.LogWarning("Tag already exists: " + tag);
                }
            }

            tagManager.ApplyModifiedProperties();
        }

        /// <summary>
        /// Removes a tag from the project settings if it exists.
        /// </summary>
        /// <param name="tag">The name of the tag to remove.</param>
        /// <param name="debugLog">If true, logs the result to the console.</param>
        public static void RemoveTag(string tag, bool debugLog = false) {
            if (string.IsNullOrEmpty(tag)) {
                Debug.LogWarning("Tag name cannot be empty.");
                return;
            }

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++) {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tag)) {
                    tagsProp.DeleteArrayElementAtIndex(i);
                    tagManager.ApplyModifiedProperties();
                    if (debugLog) {
                        Debug.Log("Tag removed: " + tag);
                    }
                    return;
                }
            }
            if (debugLog) {
                Debug.LogWarning("Tag not found: " + tag);
            }
        }
    }
}
