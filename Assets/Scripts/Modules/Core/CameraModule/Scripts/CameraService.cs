using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace vikwhite
{
    public interface ICameraService
    {
        void Initialize(Vector3 position, Quaternion rotation, float fov, Transform parent = null);
        void DetachFromParent();
        void MoveTo(Vector3 position, float duration);
        void SetDragEnabled(bool isEnabled);
    }

    public class CameraService : ICameraService, IUpdatable
    {
        private const int MaxDragRaycastHits = 32;

        private static readonly Plane FallbackDragPlane = new(Vector3.up, Vector3.zero);
        private static readonly RaycastHit[] DragRaycastHits = new RaycastHit[MaxDragRaycastHits];

        private Camera _camera;
        private Scene _originScene;
        private bool _isDragEnabled;
        private bool _isDragging;
        private Vector3 _lastDragWorldPosition;

        public CameraService()
        {
            UpdateCameraReference();
        }
        
        public void Initialize(Vector3 position, Quaternion rotation, float fov, Transform parent = null)
        {
            var camera = UpdateCameraReference();
            camera.transform.SetParent(parent);
            camera.transform.localPosition = position;
            camera.transform.localRotation = rotation;
            camera.fieldOfView = fov;
            _isDragEnabled = false;
            _isDragging = false;
        }
        
        public void DetachFromParent()
        {
            var camera = UpdateCameraReference();
            camera.transform.SetParent(null, true);
            if (_originScene.IsValid() && _originScene.isLoaded &&
                camera.gameObject.scene.handle != _originScene.handle)
                SceneManager.MoveGameObjectToScene(camera.gameObject, _originScene);
        }

        public void MoveTo(Vector3 position, float duration)
        {
            var camera = UpdateCameraReference();
            DOTween.To(
                    () => camera.transform.localPosition,
                    value => camera.transform.localPosition = value,
                    position,
                    duration)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true)
                .SetTarget(camera);
        }

        public void SetDragEnabled(bool isEnabled)
        {
            _isDragEnabled = isEnabled;
            if (!isEnabled)
                _isDragging = false;
        }

        public void Update()
        {
            if (!_isDragEnabled) return;

            var camera = UpdateCameraReference();
            var mouse = Mouse.current;
            if (camera == null || mouse == null) return;

            var screenPosition = mouse.position.ReadValue();

            if (mouse.leftButton.wasPressedThisFrame)
                BeginDrag(camera, screenPosition);

            if (!_isDragging) return;

            if (mouse.leftButton.isPressed)
                Drag(camera, screenPosition);
            else
                _isDragging = false;
        }

        private Camera UpdateCameraReference()
        {
            if (_camera == null) _camera = Camera.main;
            if (_camera != null && !_originScene.IsValid()) _originScene = _camera.gameObject.scene;
            return _camera;
        }

        private void BeginDrag(Camera camera, Vector2 screenPosition)
        {
            if (IsPointerOverUi()) return;
            if (!TryGetDragWorldPosition(camera, screenPosition, out _lastDragWorldPosition)) return;

            DOTween.Kill(camera);
            _isDragging = true;
        }

        private void Drag(Camera camera, Vector2 screenPosition)
        {
            if (!TryGetDragWorldPosition(camera, screenPosition, out var dragWorldPosition)) return;

            var height = camera.transform.position.y;
            var offset = _lastDragWorldPosition - dragWorldPosition;
            offset.y = 0f;

            var position = camera.transform.position + offset;
            position.y = height;
            camera.transform.position = position;

            if (TryGetDragWorldPosition(camera, screenPosition, out var updatedDragWorldPosition))
                _lastDragWorldPosition = updatedDragWorldPosition;
        }

        private static bool TryGetDragWorldPosition(Camera camera, Vector2 screenPosition, out Vector3 position)
        {
            var ray = camera.ScreenPointToRay(screenPosition);
            if (TryGetTerrainHit(ray, camera.farClipPlane, out var hit))
            {
                position = hit.point;
                return true;
            }

            if (FallbackDragPlane.Raycast(ray, out var distance))
            {
                position = ray.GetPoint(distance);
                return true;
            }

            position = default;
            return false;
        }

        private static bool TryGetTerrainHit(Ray ray, float maxDistance, out RaycastHit terrainHit)
        {
            var hitCount = Physics.RaycastNonAlloc(
                ray,
                DragRaycastHits,
                maxDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);

            terrainHit = default;
            var nearestDistance = float.MaxValue;

            for (var i = 0; i < hitCount; i++)
            {
                var hit = DragRaycastHits[i];
                if (hit.collider is not TerrainCollider || hit.distance >= nearestDistance) continue;

                terrainHit = hit;
                nearestDistance = hit.distance;
            }

            return nearestDistance < float.MaxValue;
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
