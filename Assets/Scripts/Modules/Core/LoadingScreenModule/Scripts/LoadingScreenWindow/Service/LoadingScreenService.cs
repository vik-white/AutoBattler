using System.Collections;
using UnityEngine;

namespace vikwhite
{
    public interface ILoadingScreenService
    {
        void Show();
        void SetProgress(float progress);
        IEnumerator TrackProgress(AsyncOperation operation);
        IEnumerator Hide();
    }

    public class LoadingScreenService : ILoadingScreenService
    {
        private const float MinVisibleTime = 1f;
        private const float FillSpeed = 1.25f;
        private const float LoadingProgressLimit = 0.95f;
        private const float TimeBasedProgressLimit = 0.85f;
        private const float ProgressCompleteThreshold = 0.999f;

        private readonly ILoadingScreenWindow _window;
        private float _shownTime;
        private float _displayedProgress;
        private float _targetProgress;
        private bool _isShown;

        public LoadingScreenService(ILoadingScreenWindow window)
        {
            _window = window;
        }

        public void Show()
        {
            _shownTime = Time.unscaledTime;
            _displayedProgress = 0f;
            _targetProgress = 0f;
            _isShown = true;
            _window.ShowWindow();
            _window.SetProgress(_displayedProgress);
        }

        public void SetProgress(float progress)
        {
            _targetProgress = Mathf.Clamp01(progress);
        }

        public IEnumerator TrackProgress(AsyncOperation operation)
        {
            if (operation == null) yield break;

            while (!operation.isDone)
            {
                SetProgress(GetLoadingTarget(operation));
                UpdateDisplayedProgress();
                yield return null;
            }
        }

        public IEnumerator Hide()
        {
            if (!_isShown) yield break;

            SetProgress(1f);
            while (Time.unscaledTime - _shownTime < MinVisibleTime ||
                   _displayedProgress < ProgressCompleteThreshold)
            {
                UpdateDisplayedProgress();
                yield return null;
            }

            _window.SetProgress(1f);
            _window.CloseWindow();
            _isShown = false;
        }

        private void UpdateDisplayedProgress()
        {
            _displayedProgress = Mathf.MoveTowards(
                _displayedProgress,
                _targetProgress,
                FillSpeed * Time.unscaledDeltaTime);
            _window.SetProgress(_displayedProgress);
        }

        private float GetLoadingTarget(AsyncOperation operation)
        {
            float sceneProgress = NormalizeSceneProgress(operation.progress);
            float elapsed = Mathf.Clamp01((Time.unscaledTime - _shownTime) / MinVisibleTime);
            float timeProgress = Mathf.SmoothStep(0f, TimeBasedProgressLimit, elapsed);

            return Mathf.Min(Mathf.Max(sceneProgress, timeProgress), LoadingProgressLimit);
        }

        private static float NormalizeSceneProgress(float progress)
        {
            return Mathf.Clamp01(progress / 0.9f);
        }
    }
}
