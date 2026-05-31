using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace vikwhite
{
    [CustomEditor(typeof(BezierPath))]
    public sealed class BezierPathEditor : Editor
    {
        private const string UndoName = "Edit Bezier Path";
        private const int TerrainRaycastSteps = 128;
        private const int TerrainRaycastBinarySteps = 12;
        private const float TerrainSurfaceTolerance = 0.03f;
        private const float RayAxisEpsilon = 0.00001f;

        private SerializedProperty closedProperty;
        private SerializedProperty samplesPerSegmentProperty;
        private SerializedProperty drawGizmosProperty;
        private SerializedProperty gizmoColorProperty;
        private SerializedProperty snapToTerrainSurfaceProperty;

        private int selectedPointIndex = -1;
        private bool showPoints = true;
        private Vector3? lastMouseGroundPosition;

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
                UpdateLastMouseGroundPosition(path);
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
                    Vector3 localPosition = TryGetLastMouseLocalPosition(path, out Vector3 mouseLocalPosition)
                        ? mouseLocalPosition
                        : GetNextPointPosition(path);
                    selectedPointIndex = path.AddPoint(localPosition);
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
                        Vector3 localPosition = TryGetLastMouseLocalPosition(path, out Vector3 mouseLocalPosition)
                            ? mouseLocalPosition
                            : GetInsertedPointPosition(path, selectedPointIndex);
                        path.InsertPoint(insertIndex, localPosition);
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

        private void UpdateLastMouseGroundPosition(BezierPath path)
        {
            Event current = Event.current;
            if (current == null) return;

            if (TryGetMouseGroundWorldPosition(path, current.mousePosition, out Vector3 worldPosition))
            {
                lastMouseGroundPosition = worldPosition;
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

        private bool TryGetLastMouseLocalPosition(BezierPath path, out Vector3 localPosition)
        {
            if (lastMouseGroundPosition.HasValue)
            {
                localPosition = path.transform.InverseTransformPoint(lastMouseGroundPosition.Value);
                return true;
            }

            localPosition = Vector3.zero;
            return false;
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
            if (TryGetMouseGroundWorldPosition(path, guiPosition, out Vector3 worldPosition))
            {
                localPosition = path.transform.InverseTransformPoint(worldPosition);
                return true;
            }

            localPosition = Vector3.zero;
            return false;
        }

        private static bool TryGetMouseGroundWorldPosition(
            BezierPath path,
            Vector2 guiPosition,
            out Vector3 worldPosition)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);

            if (TryRaycastTerrainCollider(ray, out worldPosition))
            {
                return true;
            }

            if (TryRaycastTerrainSurface(ray, out worldPosition))
            {
                return true;
            }

            if (TryRaycastSceneCollider(path, ray, out worldPosition))
            {
                return true;
            }

            return TryRaycastPathPlane(path, ray, out worldPosition);
        }

        private static bool TryRaycastTerrainCollider(Ray ray, out Vector3 worldPosition)
        {
            RaycastHit[] hits = Physics.RaycastAll(
                ray,
                Mathf.Infinity,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
            float closestDistance = float.PositiveInfinity;
            worldPosition = Vector3.zero;
            bool found = false;

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider is not TerrainCollider || hit.distance >= closestDistance) continue;

                closestDistance = hit.distance;
                worldPosition = hit.point;
                found = true;
            }

            return found;
        }

        private static bool TryRaycastTerrainSurface(Ray ray, out Vector3 worldPosition)
        {
            Terrain[] terrains = Terrain.activeTerrains;
            float closestDistance = float.PositiveInfinity;
            worldPosition = Vector3.zero;
            bool found = false;

            for (int i = 0; i < terrains.Length; i++)
            {
                if (!TryRaycastTerrainSurface(ray, terrains[i], out Vector3 hitPosition, out float hitDistance)
                    || hitDistance >= closestDistance)
                {
                    continue;
                }

                closestDistance = hitDistance;
                worldPosition = hitPosition;
                found = true;
            }

            return found;
        }

        private static bool TryRaycastTerrainSurface(
            Ray ray,
            Terrain terrain,
            out Vector3 worldPosition,
            out float hitDistance)
        {
            worldPosition = Vector3.zero;
            hitDistance = 0f;

            if (terrain == null || terrain.terrainData == null) return false;

            Vector3 terrainPosition = terrain.GetPosition();
            Vector3 terrainSize = terrain.terrainData.size;
            var bounds = new Bounds(terrainPosition + terrainSize * 0.5f, terrainSize);

            if (!TryGetRayBoundsDistanceRange(ray, bounds, out float minDistance, out float maxDistance))
            {
                return false;
            }

            minDistance = Mathf.Max(0f, minDistance);
            if (maxDistance < minDistance) return false;

            float previousDistance = minDistance;
            float previousDifference = 0f;
            bool hasPreviousSample = false;

            for (int i = 0; i <= TerrainRaycastSteps; i++)
            {
                float distance = Mathf.Lerp(minDistance, maxDistance, i / (float)TerrainRaycastSteps);
                Vector3 point = ray.GetPoint(distance);

                if (!TrySampleTerrainHeight(terrain, point, out float terrainHeight))
                {
                    continue;
                }

                float difference = point.y - terrainHeight;
                if (Mathf.Abs(difference) <= TerrainSurfaceTolerance)
                {
                    worldPosition = new Vector3(point.x, terrainHeight, point.z);
                    hitDistance = distance;
                    return true;
                }

                if (hasPreviousSample && DidCrossSurface(previousDifference, difference))
                {
                    hitDistance = RefineTerrainHitDistance(
                        ray,
                        terrain,
                        previousDistance,
                        distance,
                        previousDifference);
                    Vector3 hitPoint = ray.GetPoint(hitDistance);
                    TrySampleTerrainHeight(terrain, hitPoint, out terrainHeight);
                    worldPosition = new Vector3(hitPoint.x, terrainHeight, hitPoint.z);
                    return true;
                }

                previousDistance = distance;
                previousDifference = difference;
                hasPreviousSample = true;
            }

            return false;
        }

        private static bool TrySampleTerrainHeight(Terrain terrain, Vector3 worldPosition, out float terrainHeight)
        {
            Vector3 terrainPosition = terrain.GetPosition();
            Vector3 terrainSize = terrain.terrainData.size;
            bool insideX = worldPosition.x >= terrainPosition.x
                           && worldPosition.x <= terrainPosition.x + terrainSize.x;
            bool insideZ = worldPosition.z >= terrainPosition.z
                           && worldPosition.z <= terrainPosition.z + terrainSize.z;

            if (!insideX || !insideZ)
            {
                terrainHeight = 0f;
                return false;
            }

            float normalizedX = Mathf.InverseLerp(
                terrainPosition.x,
                terrainPosition.x + terrainSize.x,
                worldPosition.x);
            float normalizedZ = Mathf.InverseLerp(
                terrainPosition.z,
                terrainPosition.z + terrainSize.z,
                worldPosition.z);

            terrainHeight = terrain.terrainData.GetInterpolatedHeight(normalizedX, normalizedZ)
                            + terrainPosition.y;
            return true;
        }

        private static float RefineTerrainHitDistance(
            Ray ray,
            Terrain terrain,
            float lowDistance,
            float highDistance,
            float lowDifference)
        {
            for (int i = 0; i < TerrainRaycastBinarySteps; i++)
            {
                float midDistance = (lowDistance + highDistance) * 0.5f;
                Vector3 midPoint = ray.GetPoint(midDistance);

                if (!TrySampleTerrainHeight(terrain, midPoint, out float terrainHeight))
                {
                    highDistance = midDistance;
                    continue;
                }

                float midDifference = midPoint.y - terrainHeight;
                if (Mathf.Abs(midDifference) <= TerrainSurfaceTolerance)
                {
                    return midDistance;
                }

                if (DidCrossSurface(lowDifference, midDifference))
                {
                    highDistance = midDistance;
                }
                else
                {
                    lowDistance = midDistance;
                    lowDifference = midDifference;
                }
            }

            return (lowDistance + highDistance) * 0.5f;
        }

        private static bool DidCrossSurface(float firstDifference, float secondDifference)
        {
            return firstDifference < 0f && secondDifference > 0f
                   || firstDifference > 0f && secondDifference < 0f;
        }

        private static bool TryRaycastSceneCollider(BezierPath path, Ray ray, out Vector3 worldPosition)
        {
            RaycastHit[] hits = Physics.RaycastAll(
                ray,
                Mathf.Infinity,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
            float closestDistance = float.PositiveInfinity;
            worldPosition = Vector3.zero;
            bool found = false;

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider == null
                    || hit.collider.transform.IsChildOf(path.transform)
                    || hit.distance >= closestDistance)
                {
                    continue;
                }

                closestDistance = hit.distance;
                worldPosition = hit.point;
                found = true;
            }

            return found;
        }

        private static bool TryRaycastPathPlane(BezierPath path, Ray ray, out Vector3 worldPosition)
        {
            var plane = new Plane(path.transform.up, path.transform.position);

            if (plane.Raycast(ray, out float distance))
            {
                worldPosition = ray.GetPoint(distance);
                return true;
            }

            worldPosition = Vector3.zero;
            return false;
        }

        private static bool TryGetRayBoundsDistanceRange(
            Ray ray,
            Bounds bounds,
            out float minDistance,
            out float maxDistance)
        {
            minDistance = 0f;
            maxDistance = float.PositiveInfinity;

            return ClipRayAxis(ray.origin.x, ray.direction.x, bounds.min.x, bounds.max.x, ref minDistance, ref maxDistance)
                   && ClipRayAxis(ray.origin.y, ray.direction.y, bounds.min.y, bounds.max.y, ref minDistance, ref maxDistance)
                   && ClipRayAxis(ray.origin.z, ray.direction.z, bounds.min.z, bounds.max.z, ref minDistance, ref maxDistance);
        }

        private static bool ClipRayAxis(
            float origin,
            float direction,
            float min,
            float max,
            ref float minDistance,
            ref float maxDistance)
        {
            if (Mathf.Abs(direction) <= RayAxisEpsilon)
            {
                return origin >= min && origin <= max;
            }

            float enterDistance = (min - origin) / direction;
            float exitDistance = (max - origin) / direction;

            if (enterDistance > exitDistance)
            {
                (enterDistance, exitDistance) = (exitDistance, enterDistance);
            }

            minDistance = Mathf.Max(minDistance, enterDistance);
            maxDistance = Mathf.Min(maxDistance, exitDistance);
            return minDistance <= maxDistance;
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
