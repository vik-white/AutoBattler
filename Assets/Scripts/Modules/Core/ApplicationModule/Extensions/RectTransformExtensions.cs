using UnityEngine;

namespace vikwhite
{
    public static class RectTransformExtensions
    {
        public static void ClearChildren(this RectTransform rectTransform)
        {
            for (int i = rectTransform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(rectTransform.GetChild(i).gameObject);
            }
        }

        public static bool SetUiPositionFromWorld(this RectTransform rectTransform, Camera worldCamera, Vector3 worldPosition)
        {
            if (worldCamera == null || rectTransform == null)
                return false;

            var screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
            return rectTransform.SetUiPosition(screenPosition);
        }

        public static bool SetUiPosition(this RectTransform rectTransform, Vector3 screenPosition)
        {
            if (rectTransform == null || screenPosition.z < 0)
                return false;

            var parent = rectTransform.parent as RectTransform;
            if (parent == null)
            {
                rectTransform.position = screenPosition;
                return true;
            }

            var canvas = rectTransform.GetComponentInParent<Canvas>();
            var uiCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPosition, uiCamera, out var localPoint))
                return false;

            rectTransform.anchoredPosition = localPoint;
            return true;
        }
    }
}
