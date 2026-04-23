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
    }
}