using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using vikwhite.Pathing;

namespace vikwhite.EditorTools
{
    [CustomEditor(typeof(BezierPath))]
    public sealed class BezierPathEditor : Editor
    {
        private const string UndoName = "Edit Bezier Path";

        private SerializedProperty closedProperty;
        private SerializedProperty samplesPerSegmentProperty;
        private SerializedProperty drawGizmosProperty;
        private SerializedProperty gizmoColorProperty;
        private SerializedProperty snapToTerrainSurfaceProperty;

        private int selectedPointIndex = -1;
        private bool showPoints = true;

        [MenuItem("GameObject/Pathing/Bezier Path", false, 10)]
        private static void CreateBezierPath(MenuCommand menuCommand)
        {
            var gameObject = new GameObject("Bezier Path");
            GameObjectUtility.SetParentAndAlign(gameObject, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(gameObject, "Create Bezier Path");
            gameObject.AddComponent<BezierPath>();

            Selection.activeGameObject = gameObject;
        }

        private void OnEnable()
        {
            closedProperty = serializedObject.FindProperty("closed");
            samplesPerSegmentProperty = serializedObject.FindProperty("samplesPerSegment");
            drawGizmosProperty = serializedObject.FindProperty("drawGizmos");
            gizmoColorProperty = serializedObject.FindProperty("gizmoColor");
            snapToTerrainSurfaceProperty = serializedObject.FindProperty("snapToTerrainSurface");
        }

        public override void OnInspectorGUI()
        {
            var path = (BezierPath)target;

            bool wasSnapToTerrainSurface = path.SnapToTerrainSurface;

            serializedObject.Update();
            EditorGUILayout.PropertyField(closedProperty);
            EditorGUILayout.PropertyField(samplesPerSegmentProperty);
            EditorGUILayout.PropertyField(drawGizmosProperty);
            EditorGUILayout.PropertyField(gizmoColorProperty);
            EditorGUILayout.PropertyField(snapToTerrainSurfaceProperty);
            if (serializedObject.ApplyModifiedProperties())
            {
                if (!wasSnapToTerrainSurface && path.SnapToTerrainSurface)
                {
                    Record(path);
                    path.SnapAllPointsToTerrainSurface();
                }

                MarkDirty(path);
            }

            EditorGUILayout.Space(8f);
            DrawPathActions(path);
            DrawPointList(path);
        }

        private void OnSceneGUI()
        {
            var path = (BezierPath)target;
            if (path.PointCount == 0) return;

            CompareFunction previousZTest = Handles.zTest;
            Handles.zTest = CompareFunction.LessEqual;

            try
            {
                HandleSceneInput(path);
                DrawSegments(path);
                DrawPointButtons(path);

                if (IsSelectedIndexValid(path))
                {
                    DrawSelectedPointHandles(path);
                }
            }
            finally
            {
                Handles.zTest = previousZTest;
            }
        }

        private void DrawPathActions(BezierPath path)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Reset"))
                {
                    Record(path);
                    path.ResetPath();
                    selectedPointIndex = 0;
                    MarkDirty(path);
                }

                if (GUILayout.Button("Add Point"))
                {
                    Record(path);
                    selectedPointIndex = path.AddPoint(GetNextPointPosition(path));
                    MarkDirty(path);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!IsSelectedIndexValid(path)))
                {
                    if (GUILayout.Button("Insert After"))
                    {
                        Record(path);
                        int insertIndex = selectedPointIndex + 1;
                        path.InsertPoint(insertIndex, GetInsertedPointPosition(path, selectedPointIndex));
                        selectedPointIndex = insertIndex;
                        MarkDirty(path);
                    }
                }

                using (new EditorGUI.DisabledScope(!IsSelectedIndexValid(path) || path.PointCount <= 2))
                {
                    if (GUILayout.Button("Remove Selected"))
                    {
                        RemoveSelectedPoint(path);
                    }
                }
            }

            EditorGUILayout.LabelField($"Length: {path.GetLength():0.###}", EditorStyles.miniLabel);
        }

        private void DrawPointList(BezierPath path)
        {
            showPoints = EditorGUILayout.Foldout(showPoints, "Points", true);
            if (!showPoints) return;

            for (int i = 0; i < path.PointCount; i++)
            {
                BezierPathPoint point = path.GetPoint(i);

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        bool selected = GUILayout.Toggle(selectedPointIndex == i, $"Point {i}", "Button");
                        if (selected && selectedPointIndex != i)
                        {
                            selectedPointIndex = i;
                            SceneView.RepaintAll();
                        }

                        using (new EditorGUI.DisabledScope(path.PointCount <= 2))
                        {
                            if (GUILayout.Button("Remove", GUILayout.Width(72f)))
                            {
                                selectedPointIndex = i;
                                RemoveSelectedPoint(path);
                                break;
                            }
                        }
                    }

                    Vector3 position = EditorGUILayout.Vector3Field("Position", point.Position);
                    if (position != point.Position)
                    {
                        Record(path);
                        path.SetPointPosition(i, position);
                        MarkDirty(path);
                    }

                    var tangentMode = (BezierPathTangentMode)EditorGUILayout.EnumPopup("Tangent Mode", point.TangentMode);
                    if (tangentMode != point.TangentMode)
                    {
                        Record(path);
                        path.SetTangentMode(i, tangentMode);
                        MarkDirty(path);
                    }

                    float angle = EditorGUILayout.FloatField("Angle Offset", point.Angle);
                    if (!Mathf.Approximately(angle, point.Angle))
                    {
                        Record(path);
                        path.SetPointAngle(i, angle);
                        MarkDirty(path);
                    }

                    using (new EditorGUI.DisabledScope(point.TangentMode == BezierPathTangentMode.Corner))
                    {
                        Vector3 inTangent = EditorGUILayout.Vector3Field("In Tangent", point.InTangent);
                        if (inTangent != point.InTangent)
                        {
                            Record(path);
                            path.SetInTangent(i, inTangent);
                            MarkDirty(path);
                        }

                        Vector3 outTangent = EditorGUILayout.Vector3Field("Out Tangent", point.OutTangent);
                        if (outTangent != point.OutTangent)
                        {
                            Record(path);
                            path.SetOutTangent(i, outTangent);
                            MarkDirty(path);
                        }
                    }
                }
            }
        }

        private void HandleSceneInput(BezierPath path)
        {
            Event current = Event.current;

            if (current.type == EventType.KeyDown
                && (current.keyCode == KeyCode.Delete || current.keyCode == KeyCode.Backspace)
                && IsSelectedIndexValid(path)
                && path.PointCount > 2)
            {
                RemoveSelectedPoint(path);
                current.Use();
                return;
            }

            if (!current.shift) return;

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (current.type == EventType.MouseDown
                && current.button == 0
                && TryGetMouseLocalPosition(path, current.mousePosition, out Vector3 localPosition))
            {
                Record(path);
                int insertIndex = IsSelectedIndexValid(path) ? selectedPointIndex + 1 : path.PointCount;
                path.InsertPoint(insertIndex, localPosition);
                selectedPointIndex = insertIndex;
                MarkDirty(path);
                current.Use();
            }
        }

        private void DrawSegments(BezierPath path)
        {
            Color previousColor = Handles.color;
            Handles.color = path.GizmoColor;

            for (int segmentIndex = 0; segmentIndex < path.SegmentCount; segmentIndex++)
            {
                int startIndex = path.GetSegmentStartIndex(segmentIndex);
                int endIndex = path.GetSegmentEndIndex(segmentIndex);
                Vector3 start = path.GetWorldPoint(startIndex);
                Vector3 end = path.GetWorldPoint(endIndex);
                Vector3 startTangent = path.GetWorldOutTangentPoint(startIndex);
                Vector3 endTangent = path.GetWorldInTangentPoint(endIndex);

                Handles.DrawBezier(start, end, startTangent, endTangent, path.GizmoColor, null, 3f);
            }

            Handles.color = previousColor;
        }

        private void DrawPointButtons(BezierPath path)
        {
            for (int i = 0; i < path.PointCount; i++)
            {
                Vector3 worldPosition = path.GetWorldPoint(i);
                float size = HandleUtility.GetHandleSize(worldPosition) * 0.09f;
                Handles.color = selectedPointIndex == i ? Color.yellow : Color.white;

                if (Handles.Button(worldPosition, Quaternion.identity, size, size * 1.35f, Handles.SphereHandleCap))
                {
                    selectedPointIndex = i;
                    Repaint();
                }
            }
        }

        private void DrawSelectedPointHandles(BezierPath path)
        {
            BezierPathPoint point = path.GetPoint(selectedPointIndex);
            Transform pathTransform = path.transform;
            Vector3 worldPosition = path.GetWorldPoint(selectedPointIndex);
            Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local
                ? pathTransform.rotation
                : Quaternion.identity;

            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPosition = Handles.PositionHandle(worldPosition, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Record(path);
                path.SetPointPosition(selectedPointIndex, pathTransform.InverseTransformPoint(newWorldPosition));
                MarkDirty(path);
            }

            if (point.TangentMode != BezierPathTangentMode.Corner)
            {
                DrawTangentHandle(path, selectedPointIndex, true);
                DrawTangentHandle(path, selectedPointIndex, false);
            }

            DrawAngleHandle(path, selectedPointIndex);
        }

        private void DrawTangentHandle(BezierPath path, int pointIndex, bool isOutTangent)
        {
            Transform pathTransform = path.transform;
            Vector3 anchor = path.GetWorldPoint(pointIndex);
            Vector3 tangentPosition = isOutTangent
                ? path.GetWorldOutTangentPoint(pointIndex)
                : path.GetWorldInTangentPoint(pointIndex);
            Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local
                ? pathTransform.rotation
                : Quaternion.identity;

            Handles.color = isOutTangent ? new Color(0.2f, 0.9f, 0.35f) : new Color(1f, 0.45f, 0.25f);
            Handles.DrawLine(anchor, tangentPosition);

            EditorGUI.BeginChangeCheck();
            Vector3 newTangentPosition = Handles.PositionHandle(tangentPosition, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Record(path);
                Vector3 localTangent = pathTransform.InverseTransformPoint(newTangentPosition)
                                       - path.GetPoint(pointIndex).Position;

                if (isOutTangent)
                {
                    path.SetOutTangent(pointIndex, localTangent);
                }
                else
                {
                    path.SetInTangent(pointIndex, localTangent);
                }

                MarkDirty(path);
            }
        }

        private void DrawAngleHandle(BezierPath path, int pointIndex)
        {
            BezierPathPoint point = path.GetPoint(pointIndex);
            Vector3 worldPosition = path.GetWorldPoint(pointIndex);
            float size = HandleUtility.GetHandleSize(worldPosition);
            Quaternion worldRotation = path.transform.rotation * Quaternion.Euler(0f, point.Angle, 0f);
            Vector3 arrowPosition = worldPosition + path.transform.up * size * 0.18f;

            Handles.color = new Color(0.35f, 0.65f, 1f);
            Handles.ArrowHandleCap(
                0,
                arrowPosition,
                worldRotation,
                size * 0.55f,
                EventType.Repaint);

            EditorGUI.BeginChangeCheck();
            Quaternion newWorldRotation = Handles.RotationHandle(worldRotation, arrowPosition);
            if (EditorGUI.EndChangeCheck())
            {
                Record(path);
                Quaternion localRotation = Quaternion.Inverse(path.transform.rotation) * newWorldRotation;
                path.SetPointAngle(pointIndex, localRotation.eulerAngles.y);
                MarkDirty(path);
            }
        }

        private void RemoveSelectedPoint(BezierPath path)
        {
            if (!IsSelectedIndexValid(path) || path.PointCount <= 2) return;

            Record(path);
            path.RemovePoint(selectedPointIndex);
            selectedPointIndex = Mathf.Clamp(selectedPointIndex, 0, path.PointCount - 1);
            MarkDirty(path);
        }

        private bool IsSelectedIndexValid(BezierPath path)
        {
            return selectedPointIndex >= 0 && selectedPointIndex < path.PointCount;
        }

        private static Vector3 GetNextPointPosition(BezierPath path)
        {
            if (path.PointCount == 0) return Vector3.zero;
            if (path.PointCount == 1) return path.GetPoint(0).Position + Vector3.right * 2f;

            Vector3 last = path.GetPoint(path.PointCount - 1).Position;
            Vector3 previous = path.GetPoint(path.PointCount - 2).Position;
            Vector3 direction = last - previous;
            return direction.sqrMagnitude > 0.0001f ? last + direction : last + Vector3.right * 2f;
        }

        private static Vector3 GetInsertedPointPosition(BezierPath path, int selectedIndex)
        {
            Vector3 selected = path.GetPoint(selectedIndex).Position;

            if (selectedIndex + 1 < path.PointCount)
            {
                return Vector3.Lerp(selected, path.GetPoint(selectedIndex + 1).Position, 0.5f);
            }

            if (selectedIndex > 0)
            {
                Vector3 previous = path.GetPoint(selectedIndex - 1).Position;
                Vector3 direction = selected - previous;
                return direction.sqrMagnitude > 0.0001f ? selected + direction : selected + Vector3.right * 2f;
            }

            return selected + Vector3.right * 2f;
        }

        private static bool TryGetMouseLocalPosition(
            BezierPath path,
            Vector2 guiPosition,
            out Vector3 localPosition)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
            var plane = new Plane(path.transform.up, path.transform.position);

            if (plane.Raycast(ray, out float distance))
            {
                localPosition = path.transform.InverseTransformPoint(ray.GetPoint(distance));
                return true;
            }

            localPosition = Vector3.zero;
            return false;
        }

        private static void Record(BezierPath path)
        {
            Undo.RecordObject(path, UndoName);
        }

        private static void MarkDirty(BezierPath path)
        {
            EditorUtility.SetDirty(path);

            if (!Application.isPlaying && path.gameObject.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(path.gameObject.scene);
            }

            SceneView.RepaintAll();
        }
    }
}
