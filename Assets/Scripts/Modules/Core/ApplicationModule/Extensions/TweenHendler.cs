using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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

        public static Tween CreateGraphicAlphaTween(Graphic target, float endValue, float duration = 1)
        {
            return DOTween.To(() => target.color.a, alpha => SetGraphicAlpha(target, alpha), endValue, duration);
        }

        public static void SetGraphicAlpha(Graphic target, float alpha)
        {
            Color color = target.color;
            color.a = alpha;
            target.color = color;
        }
    }
}
