using System.Collections;
using UniRx;
using UnityEngine;

namespace vikwhite
{
    public interface ILoadingScreenService
    {
        IReadOnlyReactiveProperty<float> Progress { get; }
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
        private readonly ReactiveProperty<float> _progress = new(0f);
        private float _shownTime;
        private float _targetProgress;
        private bool _isShown;

        public IReadOnlyReactiveProperty<float> Progress => _progress;

        public LoadingScreenService(ILoadingScreenWindow window)
        {
            _window = window;
        }

        public void Show()
        {
            _shownTime = Time.unscaledTime;
            _progress.Value = 0f;
            _targetProgress = 0f;
            _isShown = true;
            _window.ShowWindow();
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
                   _progress.Value < ProgressCompleteThreshold)
            {
                UpdateDisplayedProgress();
                yield return null;
            }

            _progress.Value = 1f;
            _window.CloseWindow();
            _isShown = false;
        }

        private void UpdateDisplayedProgress()
        {
            _progress.Value = Mathf.MoveTowards(
                _progress.Value,
                _targetProgress,
                FillSpeed * Time.unscaledDeltaTime);
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
