using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace vikwhite
{
    public sealed class HorizontalSliderDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform _track;
        [SerializeField] private RectTransform _bar;
        [SerializeField] private RectTransform _handle;

        private Action<float> _onValueChanged;

        public void Initialize(Action<float> onValueChanged)
        {
            _onValueChanged = onValueChanged;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            UpdateValue(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateValue(eventData);
        }

        public void SetValue(float value)
        {
            if (_track == null || _bar == null || _handle == null) return;

            value = Mathf.Clamp01(value);
            float barWidth = _track.rect.width * value;
            _bar.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, barWidth);

            Vector3 trackPoint = _track.TransformPoint(
                new Vector3(Mathf.Lerp(_track.rect.xMin, _track.rect.xMax, value), 0f));
            Vector3 handlePoint = _handle.parent.InverseTransformPoint(trackPoint);
            Vector3 handlePosition = _handle.localPosition;
            handlePosition.x = handlePoint.x;
            _handle.localPosition = handlePosition;
        }

        public void Clear()
        {
            _onValueChanged = null;
        }

        private void UpdateValue(PointerEventData eventData)
        {
            if (_track == null) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _track,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPoint))
            {
                return;
            }

            float value = Mathf.InverseLerp(_track.rect.xMin, _track.rect.xMax, localPoint.x);
            _onValueChanged?.Invoke(value);
        }
    }
}
