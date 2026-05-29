using System.Collections.Generic;
using UnityEngine;

namespace vikwhite
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Pathing/Bezier Path")]
    public sealed class BezierPath : MonoBehaviour
    {
        private const int DefaultSamplesPerSegment = 16;
        private const float Epsilon = 0.0001f;

        [SerializeField] private List<BezierPathPoint> points = new List<BezierPathPoint>();
        [SerializeField] private bool closed;
        [SerializeField, Min(1)] private int samplesPerSegment = DefaultSamplesPerSegment;
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private Color gizmoColor = new Color(0.1f, 0.8f, 1f, 1f);
        [SerializeField] private bool snapToTerrainSurface;

        public IReadOnlyList<BezierPathPoint> Points => points;
        public int PointCount => points?.Count ?? 0;
        public int SegmentCount => closed && PointCount > 1 ? PointCount : Mathf.Max(0, PointCount - 1);
        public bool IsValid => SegmentCount > 0;
        public bool DrawGizmos => drawGizmos;
        public Color GizmoColor => gizmoColor;

        public bool SnapToTerrainSurface
        {
            get => snapToTerrainSurface;
            set => snapToTerrainSurface = value;
        }

        public bool Closed
        {
            get => closed;
            set => closed = value;
        }

        public int SamplesPerSegment
        {
            get => samplesPerSegment;
            set => samplesPerSegment = Mathf.Max(1, value);
        }

        public BezierPathPoint GetPoint(int index)
        {
            return points[index];
        }

        public int GetSegmentStartIndex(int segmentIndex)
        {
            ValidateSegmentIndex(segmentIndex);
            return segmentIndex;
        }

        public int GetSegmentEndIndex(int segmentIndex)
        {
            ValidateSegmentIndex(segmentIndex);
            return (segmentIndex + 1) % PointCount;
        }

        public Vector3 GetWorldPoint(int index)
        {
            return transform.TransformPoint(points[index].Position);
        }

        public Vector3 GetWorldInTangentPoint(int index)
        {
            BezierPathPoint point = points[index];
            return transform.TransformPoint(point.Position + point.InTangent);
        }

        public Vector3 GetWorldOutTangentPoint(int index)
        {
            BezierPathPoint point = points[index];
            return transform.TransformPoint(point.Position + point.OutTangent);
        }

        public void ResetPath()
        {
            points.Clear();

            var first = new BezierPathPoint(new Vector3(-1.5f, 0f, 0f))
            {
                OutTangent = new Vector3(0.5f, 0f, 0f),
                TangentMode = BezierPathTangentMode.Aligned
            };

            var second = new BezierPathPoint(new Vector3(1.5f, 0f, 0f))
            {
                InTangent = new Vector3(-0.5f, 0f, 0f),
                TangentMode = BezierPathTangentMode.Aligned
            };

            points.Add(first);
            points.Add(second);

            if (snapToTerrainSurface)
            {
                SnapAllPointsToTerrainSurface();
            }
        }

        public int AddPoint(Vector3 localPosition)
        {
            InsertPoint(PointCount, localPosition);
            return PointCount - 1;
        }

        public void InsertPoint(int insertIndex, Vector3 localPosition)
        {
            insertIndex = Mathf.Clamp(insertIndex, 0, PointCount);
            localPosition = GetSnappedLocalPosition(localPosition);

            var point = new BezierPathPoint(localPosition)
            {
                TangentMode = BezierPathTangentMode.Aligned
            };

            Vector3 previousPosition = insertIndex > 0
                ? points[insertIndex - 1].Position
                : localPosition - Vector3.right;
            Vector3 nextPosition = insertIndex < PointCount
                ? points[insertIndex].Position
                : localPosition + (localPosition - previousPosition);

            Vector3 inDirection = localPosition - previousPosition;
            Vector3 outDirection = nextPosition - localPosition;
            point.InTangent = -GetDefaultTangent(inDirection, Vector3.right);
            point.OutTangent = GetDefaultTangent(outDirection, Vector3.right);
            point.Angle = GetYawAngle(outDirection.sqrMagnitude > Epsilon ? outDirection : -inDirection);

            points.Insert(insertIndex, point);

            if (insertIndex > 0)
            {
                Vector3 previousDirection = localPosition - previousPosition;
                points[insertIndex - 1].OutTangent = previousDirection / 3f;
            }

            if (insertIndex + 1 < PointCount)
            {
                Vector3 nextDirection = points[insertIndex + 1].Position - localPosition;
                points[insertIndex + 1].InTangent = -nextDirection / 3f;
            }
        }

        public void RemovePoint(int index)
        {
            if (index < 0 || index >= PointCount) return;
            points.RemoveAt(index);
        }

        public void SetPointPosition(int index, Vector3 localPosition)
        {
            points[index].Position = GetSnappedLocalPosition(localPosition);
        }

        public void SnapAllPointsToTerrainSurface()
        {
            for (int i = 0; i < PointCount; i++)
            {
                points[i].Position = GetSnappedLocalPosition(points[i].Position);
            }
        }

        public bool TrySnapWorldPositionToTerrainSurface(Vector3 worldPosition, out Vector3 snappedWorldPosition)
        {
            Terrain[] terrains = Terrain.activeTerrains;
            for (int i = 0; i < terrains.Length; i++)
            {
                Terrain terrain = terrains[i];
                if (terrain == null || terrain.terrainData == null) continue;

                Vector3 terrainPosition = terrain.GetPosition();
                Vector3 terrainSize = terrain.terrainData.size;
                bool insideX = worldPosition.x >= terrainPosition.x
                               && worldPosition.x <= terrainPosition.x + terrainSize.x;
                bool insideZ = worldPosition.z >= terrainPosition.z
                               && worldPosition.z <= terrainPosition.z + terrainSize.z;

                if (!insideX || !insideZ) continue;

                float normalizedX = Mathf.InverseLerp(
                    terrainPosition.x,
                    terrainPosition.x + terrainSize.x,
                    worldPosition.x);
                float normalizedZ = Mathf.InverseLerp(
                    terrainPosition.z,
                    terrainPosition.z + terrainSize.z,
                    worldPosition.z);
                float terrainHeight = terrain.terrainData.GetInterpolatedHeight(normalizedX, normalizedZ)
                                      + terrainPosition.y;

                snappedWorldPosition = new Vector3(worldPosition.x, terrainHeight, worldPosition.z);
                return true;
            }

            snappedWorldPosition = worldPosition;
            return false;
        }

        public void SetPointAngle(int index, float angle)
        {
            points[index].Angle = angle;
        }

        public void SetTangentMode(int index, BezierPathTangentMode mode)
        {
            BezierPathPoint point = points[index];
            point.TangentMode = mode;
            EnforceTangentMode(index, true);
        }

        public void SetInTangent(int index, Vector3 localTangent)
        {
            points[index].InTangent = localTangent;
            EnforceTangentMode(index, false);
        }

        public void SetOutTangent(int index, Vector3 localTangent)
        {
            points[index].OutTangent = localTangent;
            EnforceTangentMode(index, true);
        }

        public Vector3 EvaluateLocalPosition(float normalizedTime)
        {
            if (!TryGetSegment(normalizedTime, out int segmentIndex, out float segmentTime))
            {
                return PointCount > 0 ? points[0].Position : Vector3.zero;
            }

            return EvaluateSegmentLocalPosition(segmentIndex, segmentTime);
        }

        public Vector3 EvaluateWorldPosition(float normalizedTime)
        {
            return transform.TransformPoint(EvaluateLocalPosition(normalizedTime));
        }

        public Vector3 EvaluateLocalTangent(float normalizedTime)
        {
            if (!TryGetSegment(normalizedTime, out int segmentIndex, out float segmentTime))
            {
                return Vector3.forward;
            }

            return EvaluateSegmentLocalTangent(segmentIndex, segmentTime);
        }

        public Vector3 EvaluateWorldTangent(float normalizedTime)
        {
            Vector3 tangent = transform.TransformVector(EvaluateLocalTangent(normalizedTime));
            return tangent.sqrMagnitude > Epsilon ? tangent.normalized : transform.forward;
        }

        public float EvaluateAngle(float normalizedTime)
        {
            if (!TryGetSegment(normalizedTime, out int segmentIndex, out float segmentTime))
            {
                return PointCount > 0 ? points[0].Angle : 0f;
            }

            int startIndex = GetSegmentStartIndex(segmentIndex);
            int endIndex = GetSegmentEndIndex(segmentIndex);
            return Mathf.LerpAngle(points[startIndex].Angle, points[endIndex].Angle, segmentTime);
        }

        public Quaternion EvaluateWorldRotation(float normalizedTime)
        {
            Vector3 tangent = EvaluateWorldTangent(normalizedTime);
            Quaternion pathRotation = tangent.sqrMagnitude > Epsilon
                ? Quaternion.LookRotation(tangent, transform.up)
                : transform.rotation;

            return pathRotation * Quaternion.Euler(0f, EvaluateAngle(normalizedTime), 0f);
        }

        public BezierPathSample GetSample(float normalizedTime, int sampleCountPerSegment = -1)
        {
            float distance = GetDistanceAt(normalizedTime, sampleCountPerSegment);
            return BuildSample(normalizedTime, distance);
        }

        public BezierPathSample GetSampleAtDistance(float distance, int sampleCountPerSegment = -1)
        {
            if (!IsValid)
            {
                return BuildSample(0f, 0f);
            }

            float length = GetLength(sampleCountPerSegment);
            if (length <= Epsilon)
            {
                return BuildSample(0f, 0f);
            }

            distance = closed ? Mathf.Repeat(distance, length) : Mathf.Clamp(distance, 0f, length);
            int samples = ResolveSampleCount(sampleCountPerSegment);
            float walkedDistance = 0f;

            for (int segmentIndex = 0; segmentIndex < SegmentCount; segmentIndex++)
            {
                Vector3 previousPosition = EvaluateSegmentWorldPosition(segmentIndex, 0f);

                for (int sampleIndex = 1; sampleIndex <= samples; sampleIndex++)
                {
                    float segmentTime = sampleIndex / (float)samples;
                    Vector3 currentPosition = EvaluateSegmentWorldPosition(segmentIndex, segmentTime);
                    float stepDistance = Vector3.Distance(previousPosition, currentPosition);

                    if (walkedDistance + stepDistance >= distance)
                    {
                        float stepRatio = stepDistance <= Epsilon
                            ? 0f
                            : (distance - walkedDistance) / stepDistance;
                        float previousTime = (sampleIndex - 1) / (float)samples;
                        float resolvedSegmentTime = Mathf.Lerp(previousTime, segmentTime, stepRatio);
                        float normalizedTime = GetNormalizedTime(segmentIndex, resolvedSegmentTime);
                        return BuildSample(normalizedTime, distance);
                    }

                    walkedDistance += stepDistance;
                    previousPosition = currentPosition;
                }
            }

            return BuildSample(closed ? 0f : 1f, distance);
        }

        public float GetLength(int sampleCountPerSegment = -1)
        {
            if (!IsValid) return 0f;

            int samples = ResolveSampleCount(sampleCountPerSegment);
            float length = 0f;

            for (int segmentIndex = 0; segmentIndex < SegmentCount; segmentIndex++)
            {
                Vector3 previousPosition = EvaluateSegmentWorldPosition(segmentIndex, 0f);

                for (int sampleIndex = 1; sampleIndex <= samples; sampleIndex++)
                {
                    Vector3 currentPosition = EvaluateSegmentWorldPosition(segmentIndex, sampleIndex / (float)samples);
                    length += Vector3.Distance(previousPosition, currentPosition);
                    previousPosition = currentPosition;
                }
            }

            return length;
        }

        public float GetDistanceAt(float normalizedTime, int sampleCountPerSegment = -1)
        {
            if (!IsValid) return 0f;

            NormalizeTime(normalizedTime, out normalizedTime);
            int samples = ResolveSampleCount(sampleCountPerSegment);
            float targetSegmentPosition = normalizedTime * SegmentCount;
            int targetSegment = Mathf.Min(Mathf.FloorToInt(targetSegmentPosition), SegmentCount - 1);
            float targetSegmentTime = targetSegmentPosition - targetSegment;
            float distance = 0f;

            for (int segmentIndex = 0; segmentIndex <= targetSegment; segmentIndex++)
            {
                float segmentEndTime = segmentIndex == targetSegment ? targetSegmentTime : 1f;
                if (segmentEndTime <= 0f) break;

                Vector3 previousPosition = EvaluateSegmentWorldPosition(segmentIndex, 0f);
                int sampleEnd = Mathf.CeilToInt(segmentEndTime * samples);

                for (int sampleIndex = 1; sampleIndex <= sampleEnd; sampleIndex++)
                {
                    float segmentTime = Mathf.Min(sampleIndex / (float)samples, segmentEndTime);
                    Vector3 currentPosition = EvaluateSegmentWorldPosition(segmentIndex, segmentTime);
                    distance += Vector3.Distance(previousPosition, currentPosition);
                    previousPosition = currentPosition;
                }
            }

            return distance;
        }

        public Vector3[] GetWorldPolyline(int sampleCountPerSegment = -1)
        {
            if (!IsValid) return new Vector3[0];

            int samples = ResolveSampleCount(sampleCountPerSegment);
            int pointCount = SegmentCount * samples + 1;
            var result = new Vector3[pointCount];
            int resultIndex = 0;

            for (int segmentIndex = 0; segmentIndex < SegmentCount; segmentIndex++)
            {
                if (segmentIndex == 0)
                {
                    result[resultIndex++] = EvaluateSegmentWorldPosition(segmentIndex, 0f);
                }

                for (int sampleIndex = 1; sampleIndex <= samples; sampleIndex++)
                {
                    result[resultIndex++] = EvaluateSegmentWorldPosition(segmentIndex, sampleIndex / (float)samples);
                }
            }

            return result;
        }

        private void Reset()
        {
            ResetPath();
        }

        private void OnValidate()
        {
            samplesPerSegment = Mathf.Max(1, samplesPerSegment);
            points ??= new List<BezierPathPoint>();

            for (int i = 0; i < points.Count; i++)
            {
                points[i] ??= new BezierPathPoint(Vector3.zero);
            }
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos || !IsValid) return;

#if UNITY_EDITOR
            UnityEngine.Rendering.CompareFunction previousZTest = UnityEditor.Handles.zTest;
            Color previousColor = UnityEditor.Handles.color;

            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            UnityEditor.Handles.color = gizmoColor;
            UnityEditor.Handles.DrawAAPolyLine(2f, GetWorldPolyline(samplesPerSegment));
            UnityEditor.Handles.zTest = previousZTest;
            UnityEditor.Handles.color = previousColor;
#else
            Gizmos.color = gizmoColor;
            Vector3[] polyline = GetWorldPolyline(samplesPerSegment);
            for (int i = 1; i < polyline.Length; i++)
            {
                Gizmos.DrawLine(polyline[i - 1], polyline[i]);
            }
#endif
        }

        private Vector3 EvaluateSegmentLocalPosition(int segmentIndex, float segmentTime)
        {
            GetSegmentPoints(
                segmentIndex,
                out Vector3 p0,
                out Vector3 p1,
                out Vector3 p2,
                out Vector3 p3);

            float u = 1f - segmentTime;
            return u * u * u * p0
                   + 3f * u * u * segmentTime * p1
                   + 3f * u * segmentTime * segmentTime * p2
                   + segmentTime * segmentTime * segmentTime * p3;
        }

        private Vector3 EvaluateSegmentWorldPosition(int segmentIndex, float segmentTime)
        {
            return transform.TransformPoint(EvaluateSegmentLocalPosition(segmentIndex, segmentTime));
        }

        private Vector3 EvaluateSegmentLocalTangent(int segmentIndex, float segmentTime)
        {
            GetSegmentPoints(
                segmentIndex,
                out Vector3 p0,
                out Vector3 p1,
                out Vector3 p2,
                out Vector3 p3);

            float u = 1f - segmentTime;
            Vector3 tangent = 3f * u * u * (p1 - p0)
                              + 6f * u * segmentTime * (p2 - p1)
                              + 3f * segmentTime * segmentTime * (p3 - p2);

            if (tangent.sqrMagnitude > Epsilon)
            {
                return tangent.normalized;
            }

            Vector3 fallback = p3 - p0;
            return fallback.sqrMagnitude > Epsilon ? fallback.normalized : Vector3.forward;
        }

        private void GetSegmentPoints(
            int segmentIndex,
            out Vector3 p0,
            out Vector3 p1,
            out Vector3 p2,
            out Vector3 p3)
        {
            int startIndex = GetSegmentStartIndex(segmentIndex);
            int endIndex = GetSegmentEndIndex(segmentIndex);
            BezierPathPoint start = points[startIndex];
            BezierPathPoint end = points[endIndex];

            p0 = start.Position;
            p1 = start.Position + start.OutTangent;
            p2 = end.Position + end.InTangent;
            p3 = end.Position;
        }

        private bool TryGetSegment(float normalizedTime, out int segmentIndex, out float segmentTime)
        {
            segmentIndex = 0;
            segmentTime = 0f;

            if (!IsValid) return false;

            NormalizeTime(normalizedTime, out normalizedTime);
            float scaledTime = normalizedTime * SegmentCount;
            segmentIndex = Mathf.Min(Mathf.FloorToInt(scaledTime), SegmentCount - 1);
            segmentTime = scaledTime - segmentIndex;
            return true;
        }

        private void NormalizeTime(float value, out float normalizedTime)
        {
            normalizedTime = closed ? Mathf.Repeat(value, 1f) : Mathf.Clamp01(value);
        }

        private float GetNormalizedTime(int segmentIndex, float segmentTime)
        {
            return SegmentCount <= 0 ? 0f : (segmentIndex + Mathf.Clamp01(segmentTime)) / SegmentCount;
        }

        private BezierPathSample BuildSample(float normalizedTime, float distance)
        {
            Vector3 position = EvaluateWorldPosition(normalizedTime);
            Vector3 tangent = EvaluateWorldTangent(normalizedTime);
            float angle = EvaluateAngle(normalizedTime);
            Quaternion rotation = EvaluateWorldRotation(normalizedTime);
            return new BezierPathSample(position, tangent, rotation, angle, distance, normalizedTime);
        }

        private int ResolveSampleCount(int sampleCountPerSegment)
        {
            return Mathf.Max(1, sampleCountPerSegment > 0 ? sampleCountPerSegment : samplesPerSegment);
        }

        private Vector3 GetSnappedLocalPosition(Vector3 localPosition)
        {
            if (!snapToTerrainSurface)
            {
                return localPosition;
            }

            Vector3 worldPosition = transform.TransformPoint(localPosition);
            return TrySnapWorldPositionToTerrainSurface(worldPosition, out Vector3 snappedWorldPosition)
                ? transform.InverseTransformPoint(snappedWorldPosition)
                : localPosition;
        }

        private void ValidateSegmentIndex(int segmentIndex)
        {
            if (segmentIndex < 0 || segmentIndex >= SegmentCount)
            {
                throw new System.ArgumentOutOfRangeException(nameof(segmentIndex));
            }
        }

        private void EnforceTangentMode(int index, bool changedOutTangent)
        {
            BezierPathPoint point = points[index];

            switch (point.TangentMode)
            {
                case BezierPathTangentMode.Corner:
                    point.InTangent = Vector3.zero;
                    point.OutTangent = Vector3.zero;
                    break;
                case BezierPathTangentMode.Mirrored:
                    if (changedOutTangent)
                    {
                        point.InTangent = -point.OutTangent;
                    }
                    else
                    {
                        point.OutTangent = -point.InTangent;
                    }
                    break;
                case BezierPathTangentMode.Aligned:
                    if (changedOutTangent)
                    {
                        point.InTangent = AlignOppositeTangent(point.InTangent, point.OutTangent);
                    }
                    else
                    {
                        point.OutTangent = AlignOppositeTangent(point.OutTangent, point.InTangent);
                    }
                    break;
            }
        }

        private static Vector3 AlignOppositeTangent(Vector3 currentOpposite, Vector3 changedTangent)
        {
            if (changedTangent.sqrMagnitude <= Epsilon) return Vector3.zero;

            float length = currentOpposite.magnitude;
            if (length <= Epsilon) length = changedTangent.magnitude;

            return -changedTangent.normalized * length;
        }

        private static Vector3 GetDefaultTangent(Vector3 direction, Vector3 fallbackDirection)
        {
            if (direction.sqrMagnitude <= Epsilon)
            {
                direction = fallbackDirection;
            }

            return direction / 3f;
        }

        private static float GetYawAngle(Vector3 localDirection)
        {
            if (localDirection.sqrMagnitude <= Epsilon) return 0f;
            return Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        }
    }
}
