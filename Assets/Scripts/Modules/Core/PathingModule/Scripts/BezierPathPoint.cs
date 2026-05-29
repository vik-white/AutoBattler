using System;
using UnityEngine;

namespace vikwhite.Pathing
{
    [Serializable]
    public sealed class BezierPathPoint
    {
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 inTangent;
        [SerializeField] private Vector3 outTangent;
        [SerializeField] private BezierPathTangentMode tangentMode = BezierPathTangentMode.Aligned;
        [SerializeField] private float angle;

        public BezierPathPoint(Vector3 position)
        {
            this.position = position;
        }

        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        public Vector3 InTangent
        {
            get => inTangent;
            set => inTangent = value;
        }

        public Vector3 OutTangent
        {
            get => outTangent;
            set => outTangent = value;
        }

        public BezierPathTangentMode TangentMode
        {
            get => tangentMode;
            set => tangentMode = value;
        }

        public float Angle
        {
            get => angle;
            set => angle = NormalizeAngle(value);
        }

        private static float NormalizeAngle(float value)
        {
            value %= 360f;
            if (value > 180f) value -= 360f;
            if (value < -180f) value += 360f;
            return value;
        }
    }
}
