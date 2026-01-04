#if UNITY_EDITOR
using AimBurst.LevelLayout.Unity;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelLayoutSpawner))]
public class LevelLayoutSpawnerEditor : Editor {
    private SerializedProperty layoutConfig;

    private GUIStyle columnHeaderStyle;

    // Editor-only range controls (1-based, inclusive)
    private int rangeFrom = 1;
    private int rangeTo = 999;

    // Copy controls
    private int copyNextCount = 1;

    private const string PrefKeyFrom = "AimBurst.LevelLayoutSpawnerEditor.RangeFrom";
    private const string PrefKeyTo = "AimBurst.LevelLayoutSpawnerEditor.RangeTo";
    private const string PrefKeyCopyNextCount = "AimBurst.LevelLayoutSpawnerEditor.CopyNextCount";

    private void OnEnable() {
        layoutConfig = serializedObject.FindProperty("layoutConfig");
        rangeFrom = EditorPrefs.GetInt(PrefKeyFrom, 1);
        rangeTo = EditorPrefs.GetInt(PrefKeyTo, 999);
        copyNextCount = Mathf.Max(1, EditorPrefs.GetInt(PrefKeyCopyNextCount, 1));
    }

    public override void OnInspectorGUI() {
        columnHeaderStyle ??= new GUIStyle(EditorStyles.boldLabel) {
            fontSize = 14,
            alignment = TextAnchor.LowerCenter
        };

        serializedObject.Update();

        DrawDisabledScriptField();

        // Draw everything except layoutConfig (we custom-draw it)
        DrawPropertiesExcluding(serializedObject, "m_Script", "layoutConfig");

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Layout Config", EditorStyles.boldLabel);
        EditorGUILayout.Space(6);

        if (layoutConfig == null) {
            EditorGUILayout.HelpBox("Could not find serialized property: layoutConfig", MessageType.Error);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        DrawRangeControls(layoutConfig.arraySize);

        int min = Mathf.Clamp(rangeFrom, 1, layoutConfig.arraySize) - 1;
        int max = Mathf.Clamp(rangeTo, 1, layoutConfig.arraySize) - 1;
        if (min > max) (min, max) = (max, min);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField(
            $"Showing columns {min + 1} - {max + 1} of {layoutConfig.arraySize}",
            EditorStyles.miniLabel
        );
        EditorGUILayout.Space(6);

        for (int i = min; i <= max; i++) {
            var element = layoutConfig.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField($"Column {i + 1}", columnHeaderStyle);
            EditorGUILayout.Space(5);

            var setsProp = element.FindPropertyRelative("sets");
            EditorGUILayout.PropertyField(setsProp, includeChildren: true);

            EditorGUILayout.Space(10);

            DrawCopyControls(i, layoutConfig.arraySize);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(50);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCopyControls(int sourceIndex, int arraySize) {
        EditorGUILayout.LabelField("Copy", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        copyNextCount = EditorGUILayout.IntField("Copy Count (next N)", copyNextCount);
        copyNextCount = Mathf.Max(0, copyNextCount);
        if (EditorGUI.EndChangeCheck()) {
            EditorPrefs.SetInt(PrefKeyCopyNextCount, copyNextCount);
            Repaint();
        }

        int remaining = Mathf.Max(0, (arraySize - 1) - sourceIndex); // how many slots exist after this index
        int effective = Mathf.Min(copyNextCount, remaining);

        using (new EditorGUI.DisabledScope(effective <= 0)) {
            if (GUILayout.Button($"Copy To Next {effective} (Overwrite)")) {
                // Commit any edits before copying.
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                CopyElementOverwriteNextN(sourceIndex, copyNextCount);

                // Push changes to the object.
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                Repaint();
            }
        }

        if (copyNextCount > 0 && remaining > 0 && effective != copyNextCount) {
            EditorGUILayout.HelpBox(
                $"Clamped: only {remaining} columns exist after this one, so it will copy to next {effective}.",
                MessageType.Info
            );
        }
    }

    private void DrawDisabledScriptField() {
        // Show Script as "Disabled" (read-only)
        using (new EditorGUI.DisabledScope(true)) {
            var mb = (MonoBehaviour)target;
            var script = MonoScript.FromMonoBehaviour(mb);
            EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
        }
    }

    private void CopyElementOverwriteNextN(int sourceIndex, int count) {
        int arraySize = layoutConfig.arraySize;
        if (sourceIndex < 0 || sourceIndex >= arraySize) return;
        if (count <= 0) return;

        int lastDest = Mathf.Min(arraySize - 1, sourceIndex + count);
        if (lastDest <= sourceIndex) return;

        Undo.RecordObject(target, $"Copy Column To Next {lastDest - sourceIndex} (Overwrite)");

        var src = layoutConfig.GetArrayElementAtIndex(sourceIndex);

        for (int destIndex = sourceIndex + 1; destIndex <= lastDest; destIndex++) {
            var dst = layoutConfig.GetArrayElementAtIndex(destIndex);
            CopyPropertyRecursive(src, dst);
        }
    }

    private static void CopyPropertyRecursive(SerializedProperty src, SerializedProperty dst) {
        if (src == null || dst == null) return;

        // If they’re fundamentally different, bail.
        if (src.propertyType != dst.propertyType) return;

        switch (src.propertyType) {
            case SerializedPropertyType.Integer: dst.intValue = src.intValue; break;
            case SerializedPropertyType.Boolean: dst.boolValue = src.boolValue; break;
            case SerializedPropertyType.Float: dst.floatValue = src.floatValue; break;
            case SerializedPropertyType.String: dst.stringValue = src.stringValue; break;
            case SerializedPropertyType.Color: dst.colorValue = src.colorValue; break;
            case SerializedPropertyType.ObjectReference: dst.objectReferenceValue = src.objectReferenceValue; break;
            case SerializedPropertyType.LayerMask: dst.intValue = src.intValue; break;
            case SerializedPropertyType.Enum: dst.enumValueIndex = src.enumValueIndex; break;
            case SerializedPropertyType.Vector2: dst.vector2Value = src.vector2Value; break;
            case SerializedPropertyType.Vector3: dst.vector3Value = src.vector3Value; break;
            case SerializedPropertyType.Vector4: dst.vector4Value = src.vector4Value; break;
            case SerializedPropertyType.Rect: dst.rectValue = src.rectValue; break;
            case SerializedPropertyType.ArraySize: dst.intValue = src.intValue; break;
            case SerializedPropertyType.Character: dst.intValue = src.intValue; break;
            case SerializedPropertyType.AnimationCurve: dst.animationCurveValue = src.animationCurveValue; break;
            case SerializedPropertyType.Bounds: dst.boundsValue = src.boundsValue; break;
            case SerializedPropertyType.Quaternion: dst.quaternionValue = src.quaternionValue; break;
            case SerializedPropertyType.ExposedReference: dst.exposedReferenceValue = src.exposedReferenceValue; break;
            case SerializedPropertyType.FixedBufferSize: dst.intValue = src.intValue; break;
            case SerializedPropertyType.Vector2Int: dst.vector2IntValue = src.vector2IntValue; break;
            case SerializedPropertyType.Vector3Int: dst.vector3IntValue = src.vector3IntValue; break;
            case SerializedPropertyType.RectInt: dst.rectIntValue = src.rectIntValue; break;
            case SerializedPropertyType.BoundsInt: dst.boundsIntValue = src.boundsIntValue; break;
#if UNITY_2020_2_OR_NEWER
            case SerializedPropertyType.ManagedReference:
                dst.managedReferenceValue = src.managedReferenceValue;
                break;
#endif
            case SerializedPropertyType.Generic:
                if (src.isArray && src.propertyType != SerializedPropertyType.String) {
                    // Copy arrays (including nested arrays) by resizing destination to match source.
                    dst.arraySize = src.arraySize;
                    for (int i = 0; i < src.arraySize; i++) {
                        var srcEl = src.GetArrayElementAtIndex(i);
                        var dstEl = dst.GetArrayElementAtIndex(i);
                        CopyPropertyRecursive(srcEl, dstEl);
                    }
                }
                else {
                    // Copy each child field
                    var srcIter = src.Copy();
                    var srcEnd = srcIter.GetEndProperty();

                    bool enterChildren = true;
                    while (srcIter.NextVisible(enterChildren) && !SerializedProperty.EqualContents(srcIter, srcEnd)) {
                        enterChildren = false;

                        var dstChild = dst.FindPropertyRelative(srcIter.name);
                        if (dstChild != null) {
                            CopyPropertyRecursive(srcIter, dstChild);
                        }
                    }
                }
                break;

            default:
                // Some types we ignore safely.
                break;
        }
    }

    private void DrawRangeControls(int arraySize) {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("Visible Columns Range", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        rangeFrom = EditorGUILayout.IntField("From (1-based)", rangeFrom);
        rangeTo = EditorGUILayout.IntField("To (1-based)", rangeTo);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("All")) {
            rangeFrom = 1;
            rangeTo = arraySize;
        }
        if (GUILayout.Button("First 5")) {
            rangeFrom = 1;
            rangeTo = Mathf.Min(5, arraySize);
        }
        if (GUILayout.Button("Last 5")) {
            rangeFrom = Mathf.Max(1, arraySize - 4);
            rangeTo = arraySize;
        }
        EditorGUILayout.EndHorizontal();

        rangeFrom = Mathf.Clamp(rangeFrom, 1, Mathf.Max(1, arraySize));
        rangeTo = Mathf.Clamp(rangeTo, 1, Mathf.Max(1, arraySize));

        if (EditorGUI.EndChangeCheck()) {
            EditorPrefs.SetInt(PrefKeyFrom, rangeFrom);
            EditorPrefs.SetInt(PrefKeyTo, rangeTo);
            Repaint();
        }

        EditorGUILayout.EndVertical();
    }
}
#endif
