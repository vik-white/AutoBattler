using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class SectorPoint : MonoBehaviour
    {
        private const float GizmoRadius = 0.1f;
        private const float GizmoArrowLength = 0.5f;
        private const float GizmoArrowHeadLength = 0.12f;
        private const float GizmoArrowHeadAngle = 25f;
        private const float CharacterScale = 1.5f;

        public int Index;
        public BezierPath Path;

        private GameObject _characterPrefab;
        private GameObject _characterInstance;

        public void Initialize(IMapData mapData, bool showCharacter)
        {
            gameObject.SetActive(true);
            SetCharacterPrefab(mapData?.Prefab);
            SetCharacterVisible(showCharacter);
        }

        public void SetCharacterVisible(bool visible)
        {
            if (_characterInstance != null)
            {
                _characterInstance.SetActive(visible);
            }
        }

        private void SetCharacterPrefab(GameObject prefab)
        {
            if (_characterPrefab == prefab && _characterInstance != null)
            {
                AlignCharacter();
                return;
            }

            ClearCharacter();
            _characterPrefab = prefab;

            if (_characterPrefab == null) return;

            _characterInstance = Instantiate(_characterPrefab, transform.position, transform.rotation, transform);
            _characterInstance.name = _characterPrefab.name;
            AlignCharacter();
        }

        private void AlignCharacter()
        {
            if (_characterInstance == null) return;

            _characterInstance.transform.SetPositionAndRotation(transform.position, transform.rotation);
            _characterInstance.transform.localScale = Vector3.one * CharacterScale;
        }

        private void ClearCharacter()
        {
            if (_characterInstance != null)
            {
                Destroy(_characterInstance);
            }

            _characterPrefab = null;
            _characterInstance = null;
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
