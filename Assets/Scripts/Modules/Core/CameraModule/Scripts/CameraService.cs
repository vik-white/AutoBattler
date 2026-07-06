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
        void SetLobbyControlsEnabled(bool isEnabled);
    }

    public class CameraService : ICameraService, IUpdatable
    {
        private const int MaxDragRaycastHits = 32;
        private const float MouseWheelZoomSensitivity = 1.5f;
        private const float TouchZoomSensitivity = 0.03f;
        private const float MaxZoomInMultiplier = 0.5f;
        private const float MaxZoomOutMultiplier = 1.5f;

        private static readonly Plane FallbackDragPlane = new(Vector3.up, Vector3.zero);
        private static readonly RaycastHit[] DragRaycastHits = new RaycastHit[MaxDragRaycastHits];

        private Camera _camera;
        private Scene _originScene;
        private bool _isLobbyControlsEnabled;
        private bool _isDragging;
        private bool _isPinching;
        private float _baseFov;
        private float _lastPinchDistance;
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
            _baseFov = fov;
            _isLobbyControlsEnabled = false;
            _isDragging = false;
            _isPinching = false;
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

        public void SetLobbyControlsEnabled(bool isEnabled)
        {
            _isLobbyControlsEnabled = isEnabled;
            if (!isEnabled)
            {
                _isDragging = false;
                _isPinching = false;
            }
        }

        public void Update()
        {
            if (!_isLobbyControlsEnabled) return;

            var camera = UpdateCameraReference();
            if (camera == null) return;

            UpdateMouseControls(camera);
            UpdateTouchControls(camera);
        }

        private void UpdateMouseControls(Camera camera)
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            var scroll = mouse.scroll.ReadValue().y;
            if (!Mathf.Approximately(scroll, 0f))
                ApplyZoom(camera, -scroll * MouseWheelZoomSensitivity);

            if (mouse.leftButton.wasPressedThisFrame)
                BeginDrag(camera, mouse.position.ReadValue());

            if (!_isDragging) return;

            if (mouse.leftButton.isPressed)
                Drag(camera, mouse.position.ReadValue());
            else
                _isDragging = false;
        }

        private void UpdateTouchControls(Camera camera)
        {
            var touchscreen = Touchscreen.current;
            if (touchscreen == null) return;

            if (!TryGetTwoTouches(touchscreen, out var firstPosition, out var secondPosition))
            {
                _isPinching = false;
                return;
            }

            _isDragging = false;
            var pinchDistance = Vector2.Distance(firstPosition, secondPosition);
            if (_isPinching)
                ApplyZoom(camera, (_lastPinchDistance - pinchDistance) * TouchZoomSensitivity);

            _lastPinchDistance = pinchDistance;
            _isPinching = true;
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

        private void ApplyZoom(Camera camera, float fovDelta)
        {
            var baseFov = _baseFov > 0f ? _baseFov : camera.fieldOfView;
            camera.fieldOfView = Mathf.Clamp(
                camera.fieldOfView + fovDelta,
                baseFov * MaxZoomInMultiplier,
                baseFov * MaxZoomOutMultiplier);
        }

        private static bool TryGetTwoTouches(Touchscreen touchscreen, out Vector2 firstPosition, out Vector2 secondPosition)
        {
            firstPosition = default;
            secondPosition = default;
            var touchCount = 0;

            foreach (var touch in touchscreen.touches)
            {
                if (!touch.press.isPressed) continue;

                if (touchCount == 0)
                    firstPosition = touch.position.ReadValue();
                else if (touchCount == 1)
                    secondPosition = touch.position.ReadValue();

                touchCount++;
                if (touchCount >= 2)
                    return true;
            }

            return false;
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
