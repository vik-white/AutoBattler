using TMPro;
using UnityEngine;

namespace vikwhite
{
    public class SectorPoint : MonoBehaviour
    {
        private const float GizmoRadius = 0.1f;
        private const float GizmoArrowLength = 0.5f;
        private const float GizmoArrowHeadLength = 0.12f;
        private const float GizmoArrowHeadAngle = 25f;

        public int Index;

        public Vector3 Position => transform.position;

        public void Initialize()
        {
            gameObject.SetActive(true);
        }

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            Vector3 forward = transform.forward;
            Color previousColor = Gizmos.color;

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(position, GizmoRadius);
            DrawForwardArrow(position, forward);
            Gizmos.color = previousColor;

#if UNITY_EDITOR
            UnityEditor.Handles.Label(position + Vector3.up * (GizmoRadius * 1.5f), Index.ToString());
#endif
        }

        private static void DrawForwardArrow(Vector3 position, Vector3 forward)
        {
            Vector3 direction = forward.normalized;
            Vector3 tip = position + direction * GizmoArrowLength;

            Gizmos.DrawLine(position, tip);

            Quaternion rightRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f + GizmoArrowHeadAngle, 0f);
            Quaternion leftRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 180f - GizmoArrowHeadAngle, 0f);

            Gizmos.DrawRay(tip, rightRotation * Vector3.forward * GizmoArrowHeadLength);
            Gizmos.DrawRay(tip, leftRotation * Vector3.forward * GizmoArrowHeadLength);
        }
    }
}
