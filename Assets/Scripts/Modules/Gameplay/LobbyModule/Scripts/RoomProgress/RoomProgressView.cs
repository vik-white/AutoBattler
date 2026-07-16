using UniRx;
using UnityEngine;
using UnityEngine.UI;
using vikwhite.RoomProgress;

namespace vikwhite
{
    public class RoomProgressView : View<RoomProgressHierarchy, RoomProgressViewModel>
    {
        private readonly RectTransform _rectTransform;

        public RoomProgressView(GameObject view) : base(view)
        {
            _rectTransform = view.transform as RectTransform;
        }

        protected override void UpdateViewModel(RoomProgressViewModel viewModel)
        {
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _rectTransform.pivot = new Vector2(0.5f, 0.5f);
            _rectTransform.localScale = Vector3.one;

            Register(Observable.EveryLateUpdate().Subscribe(_ => UpdateView()));
            UpdateView();
        }

        private void UpdateView()
        {
            if (BaseViewModel == null)
            {
                SetActive(false);
                return;
            }

            var state = BaseViewModel.GetState();
            if (!state.IsUpgrading)
            {
                SetActive(false);
                return;
            }

            var camera = Camera.main;
            if (camera == null || !BaseViewModel.TryGetWorldPosition(out var worldPosition))
            {
                SetActive(false);
                return;
            }

            var screenPosition = camera.WorldToScreenPoint(worldPosition);
            var isVisible = screenPosition.z > 0;
            SetActive(isVisible);
            if (!isVisible) return;

            _rectTransform.SetUiPosition(screenPosition);
            _view.ProgressBar.SetProgress(state.Progress);
            _view.ProgressBar.SetText(FormatRemainingTime(state.SecondsRemaining));
        }

        private static string FormatRemainingTime(long totalSeconds)
        {
            var hours = totalSeconds / 3600;
            var minutes = totalSeconds % 3600 / 60;
            var seconds = totalSeconds % 60;
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }
    }
}
