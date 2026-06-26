using DG.Tweening;
using UnityEngine;

namespace vikwhite
{
    public static class TweenHendler
    {
        public static Tween CreateAnchoredPositionYTween(RectTransform target, float endValue, float duration = 1)
        {
            return DOTween.To(
                () => target.anchoredPosition.y,
                y =>
                {
                    Vector2 position = target.anchoredPosition;
                    position.y = y;
                    target.anchoredPosition = position;
                },
                endValue,
                duration);
        }
    }
}