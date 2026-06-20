using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    [RequireComponent(typeof(RectTransform))]
    public class UILayoutAutoRebuilder : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private int _rebuildFrames = 2;
        [SerializeField] private bool _rebuildParent = true;

        private RectTransform _rectTransform;
        private Coroutine _rebuildCoroutine;

        private RectTransform Target => _target != null ? _target : _rectTransform ??= GetComponent<RectTransform>();

        private void OnEnable()
        {
            ScheduleRebuild();
        }

        private void OnTransformChildrenChanged()
        {
            ScheduleRebuild();
        }

        public void ScheduleRebuild()
        {
            if (!isActiveAndEnabled) return;
            if (_rebuildCoroutine != null) StopCoroutine(_rebuildCoroutine);
            _rebuildCoroutine = StartCoroutine(RebuildRoutine());
        }

        public void RebuildNow()
        {
            Canvas.ForceUpdateCanvases();
            Rebuild(Target);

            if (_rebuildParent && Target.parent is RectTransform parent)
                Rebuild(parent);

            Canvas.ForceUpdateCanvases();
        }

        private IEnumerator RebuildRoutine()
        {
            var frames = Mathf.Max(1, _rebuildFrames);

            for (int i = 0; i < frames; i++)
            {
                yield return null;
                RebuildNow();
            }

            _rebuildCoroutine = null;
        }

        private void Rebuild(RectTransform rectTransform)
        {
            if (rectTransform == null) return;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}
