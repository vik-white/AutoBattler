using UnityEngine;

namespace vikwhite
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _bar;

        private float _fullWidth;

        private void Awake()
        {
            CacheFullWidth();
        }

        public void SetProgress(float value)
        {
            CacheFullWidth();

            value = Mathf.Clamp01(value);
            _bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _fullWidth * value);
        }

        private void CacheFullWidth()
        {
            if (_bar == null) _bar = transform as RectTransform;
            if (_bar == null || _fullWidth > 0) return;

            _fullWidth = _bar.rect.width;
            if (_fullWidth <= 0) _fullWidth = _bar.sizeDelta.x;
        }
    }
}
