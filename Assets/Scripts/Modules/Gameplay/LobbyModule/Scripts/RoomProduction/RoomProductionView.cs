using UniRx;
using UnityEngine;
using UnityEngine.UI;
using vikwhite.Data;

namespace vikwhite
{
    public class RoomProductionView : View<RoomProductionHierarchy, RoomProductionViewModel>
    {
        private readonly IConfigs _configs;
        private readonly RectTransform _rectTransform;
        private readonly Button _button;

        public RoomProductionView(GameObject view, IConfigs configs) : base(view)
        {
            _configs = configs;
            _rectTransform = view.transform as RectTransform;
            _button = view.GetComponent<Button>();
        }

        protected override void UpdateViewModel(RoomProductionViewModel viewModel)
        {
            GameObject.name = $"RoomProduction_{viewModel.Type}";
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _rectTransform.pivot = new Vector2(0.5f, 0.5f);
            _rectTransform.localScale = Vector3.one;

            var canvasGroup = GameObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            if (_configs.UI.ResourceIcons.TryGetValue(viewModel.Type, out var icon))
                _view.Icon.sprite = icon;
            _view.Icon.preserveAspect = true;
            _view.Time.gameObject.SetActive(true);

            if (_button != null) BindClick(_button, viewModel.OnCollect);
            Register(Observable.EveryLateUpdate().Subscribe(_ => UpdateView()));
            UpdateView();
        }

        private void UpdateView()
        {
            if (BaseViewModel == null || !BaseViewModel.HasProduction)
            {
                SetActive(false);
                return;
            }

            UpdatePosition();
            UpdateProduction();
        }

        private void UpdatePosition()
        {
            var camera = Camera.main;
            if (BaseViewModel == null
                || camera == null
                || !BaseViewModel.TryGetWorldPosition(out var worldPosition))
            {
                SetActive(false);
                return;
            }

            var screenPosition = camera.WorldToScreenPoint(worldPosition);
            var isVisible = screenPosition.z > 0;
            SetActive(isVisible);
            if (isVisible) _rectTransform.SetUiPosition(screenPosition);
        }

        private void UpdateProduction()
        {
            if (BaseViewModel == null) return;

            var state = BaseViewModel.GetState();
            if (_button != null) _button.interactable = state.CollectibleAmount > 0;
            var rounded = Mathf.Round(state.Accumulated);
            _view.Value.text = Mathf.Approximately(state.Accumulated, rounded)
                ? Mathf.RoundToInt(state.Accumulated).ToString()
                : $"{state.Accumulated:0.#}";
            _view.Time.text = $"{state.SecondsUntilNextProduction / 60:00}:" +
                              $"{state.SecondsUntilNextProduction % 60:00}";
        }
    }
}
