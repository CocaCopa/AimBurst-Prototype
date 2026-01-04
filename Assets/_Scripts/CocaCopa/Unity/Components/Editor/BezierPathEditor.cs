using System.Collections.Generic;
using CocaCopa.EditorUtils;
using CocaCopa.Unity.Components.Bezier;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.Unity.Components.EditorTools {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BezierPath))]
    public class BezierPathEditor : Editor {
        // Reference Points
        private SerializedProperty start;
        private SerializedProperty end;

        // Control Points
        private SerializedProperty upHint;
        private SerializedProperty control1Along;
        private SerializedProperty control1Offset;
        private SerializedProperty control2Along;
        private SerializedProperty control2Offset;

        // Movement
        private SerializedProperty targetTransform;
        private SerializedProperty followOffset;
        private SerializedProperty deltaTime;
        private SerializedProperty movementMode;
        private SerializedProperty duration;
        private SerializedProperty speed;
        private SerializedProperty animationMode;
        private SerializedProperty loopMode;
        private SerializedProperty lookAtPath;

        private BezierPath path;

        private int segments = 32;
        private bool drawControlPoints = true;
        private bool colorsFoldout;
        private Color curveColor = Color.cyan;
        private Color controlLinesColor = Color.white;
        private Color controlPointsColor = Color.yellow;

        private bool controlPointFoldout_1;
        private bool controlPointFoldout_2;

        private void OnEnable() {
            path = target as BezierPath;
            FindProperties();
            LoadEditorPrefs();
        }

        private void FindProperties() {
            start = serializedObject.FindProperty(nameof(start));
            end = serializedObject.FindProperty(nameof(end));

            upHint = serializedObject.FindProperty(nameof(upHint));
            control1Along = serializedObject.FindProperty(nameof(control1Along));
            control1Offset = serializedObject.FindProperty(nameof(control1Offset));
            control2Along = serializedObject.FindProperty(nameof(control2Along));
            control2Offset = serializedObject.FindProperty(nameof(control2Offset));

            targetTransform = serializedObject.FindProperty(nameof(targetTransform));
            followOffset = serializedObject.FindProperty(nameof(followOffset));
            deltaTime = serializedObject.FindProperty(nameof(deltaTime));
            movementMode = serializedObject.FindProperty(nameof(movementMode));
            duration = serializedObject.FindProperty(nameof(duration));
            speed = serializedObject.FindProperty(nameof(speed));
            animationMode = serializedObject.FindProperty(nameof(animationMode));
            loopMode = serializedObject.FindProperty(nameof(loopMode));
            lookAtPath = serializedObject.FindProperty(nameof(lookAtPath));
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorCommon.DisplayScriptReference(target);

            EditorGUI.BeginChangeCheck();
            DrawVisualizationProperties();
            if (EditorGUI.EndChangeCheck()) {
                SceneView.RepaintAll();
                SaveEditorPrefs();
            }

            DrawReferencePointsProperties();
            DrawControlPointPlacementProperties();
            DrawMovementProperties();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawVisualizationProperties() {
            EditorGUILayout.Space(5);

            EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
            string label = serializedObject.isEditingMultipleObjects
                ? "Visualization (Multi-editing not supported)"
                : "Visualization";
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            segments = (int)EditorGUILayout.Slider("Segments", segments, 2, 64);
            drawControlPoints = EditorGUILayout.Toggle("Draw Control Points", drawControlPoints);
            colorsFoldout = EditorGUILayout.Foldout(colorsFoldout, "Colors", toggleOnLabelClick: true);
            if (colorsFoldout) {
                EditorGUI.indentLevel++;
                curveColor = EditorGUILayout.ColorField("Curve", curveColor);
                controlLinesColor = EditorGUILayout.ColorField("Control Lines", controlLinesColor);
                controlPointsColor = EditorGUILayout.ColorField("Control Points", controlPointsColor);
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(5);
        }

        private void DrawReferencePointsProperties() {
            EditorGUILayout.LabelField("Reference Points", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(start);
            EditorGUILayout.PropertyField(end);
            EditorGUILayout.Space(5);
        }

        private void DrawControlPointPlacementProperties() {
            EditorGUILayout.LabelField("Control Points", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(upHint);

            EditorGUILayout.LabelField("Point 1");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(control1Along, new GUIContent("Along"));
            EditorGUILayout.PropertyField(control1Offset, new GUIContent("Offset"));
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Point 2");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(control2Along, new GUIContent("Along"));
            EditorGUILayout.PropertyField(control2Offset, new GUIContent("Offset"));
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(5);
        }

        private void DrawMovementProperties() {
            EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetTransform);

            EditorGUILayout.LabelField("Path");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(followOffset);
            EditorGUILayout.PropertyField(lookAtPath);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Movement");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(deltaTime);
            EditorGUILayout.PropertyField(movementMode, new GUIContent("Mode"));
            if (movementMode.enumValueIndex == 0) {
                EditorGUILayout.PropertyField(duration);
            }
            else if (movementMode.enumValueIndex == 1) {
                EditorGUILayout.PropertyField(speed);
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Animation");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(animationMode, new GUIContent("Mode"));
            EditorGUILayout.PropertyField(loopMode, new GUIContent("Loop"));
            EditorGUI.indentLevel--;
        }

        private void OnSceneGUI() {
            if (path.Start == null || path.End == null) return;

            var points = path.GetBezierPoints();

            // Curve
            Handles.color = curveColor;

            Vector3 prev = points.A;
            int segs = Mathf.Max(2, segments);

            for (int i = 1; i <= segs; i++) {
                float t = i / (float)segs;
                Vector3 p = BezierPathMath.CubicBezier(points.A, points.B, points.C, points.D, t);
                Handles.DrawLine(prev, p);
                prev = p;
            }

            if (!drawControlPoints) return;

            // Control Lines
            Handles.color = controlLinesColor;
            Handles.DrawLine(points.A, points.B);
            Handles.DrawLine(points.C, points.D);

            // Control Points
            Handles.color = controlPointsColor;
            Handles.SphereHandleCap(0, points.B, Quaternion.identity, 0.16f, EventType.Repaint);
            Handles.SphereHandleCap(0, points.C, Quaternion.identity, 0.16f, EventType.Repaint);
        }

        private void SaveEditorPrefs() {
            EditorReg.Save(target, nameof(segments), segments);
            EditorReg.Save(target, nameof(drawControlPoints), drawControlPoints);
            EditorReg.Save(target, nameof(controlPointFoldout_1), controlPointFoldout_1);
            EditorReg.Save(target, nameof(controlPointFoldout_2), controlPointFoldout_2);
            EditorReg.Save(target, nameof(colorsFoldout), colorsFoldout);
            EditorReg.Save(target, nameof(curveColor), curveColor);
            EditorReg.Save(target, nameof(controlLinesColor), controlLinesColor);
            EditorReg.Save(target, nameof(controlPointsColor), controlPointsColor);
        }

        private void LoadEditorPrefs() {
            segments = EditorReg.Load(target, nameof(segments), 32);
            drawControlPoints = EditorReg.Load(target, nameof(drawControlPoints), true);
            controlPointFoldout_1 = EditorReg.Load(target, nameof(controlPointFoldout_1), true);
            controlPointFoldout_2 = EditorReg.Load(target, nameof(controlPointFoldout_2), true);
            colorsFoldout = EditorReg.Load(target, nameof(colorsFoldout), false);
            curveColor = EditorReg.Load(target, nameof(curveColor), Color.cyan);
            controlLinesColor = EditorReg.Load(target, nameof(controlLinesColor), Color.white);
            controlPointsColor = EditorReg.Load(target, nameof(controlPointsColor), Color.yellow);
        }
    }
}
