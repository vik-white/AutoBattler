using UnityEngine;

namespace vikwhite
{
    public readonly struct BezierPathSample
    {
        public readonly Vector3 Position;
        public readonly Vector3 Tangent;
        public readonly Quaternion Rotation;
        public readonly float Angle;
        public readonly float Distance;
        public readonly float NormalizedTime;

        public BezierPathSample(
            Vector3 position,
            Vector3 tangent,
            Quaternion rotation,
            float angle,
            float distance,
            float normalizedTime)
        {
            Position = position;
            Tangent = tangent;
            Rotation = rotation;
            Angle = angle;
            Distance = distance;
            NormalizedTime = normalizedTime;
        }
    }
}
