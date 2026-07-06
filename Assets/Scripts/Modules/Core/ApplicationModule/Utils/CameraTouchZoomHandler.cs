using UnityEngine;
using UnityEngine.InputSystem;

namespace vikwhite
{
    public static class CameraTouchZoomHandler
    {
        private const float ZoomSensitivity = 0.03f;

        private static bool _isPinching;
        private static float _lastPinchDistance;

        public static void Reset()
        {
            _isPinching = false;
            _lastPinchDistance = 0f;
        }

        public static bool TryUpdateZoom(out float zoomDelta, out bool isGestureActive)
        {
            zoomDelta = 0f;
            isGestureActive = false;

            var touchscreen = Touchscreen.current;
            if (touchscreen == null || !TryGetTwoTouches(touchscreen, out var firstPosition, out var secondPosition))
            {
                Reset();
                return false;
            }

            isGestureActive = true;
            var pinchDistance = Vector2.Distance(firstPosition, secondPosition);
            if (_isPinching)
                zoomDelta = (_lastPinchDistance - pinchDistance) * ZoomSensitivity;

            _lastPinchDistance = pinchDistance;
            _isPinching = true;

            return !Mathf.Approximately(zoomDelta, 0f);
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
    }
}
