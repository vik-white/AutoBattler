using UniRx;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class RoomProductionView : View<RoomProductionHierarchy, RoomProductionViewModel>
    {
        private readonly IConfigs _configs;
        private readonly RectTransform _rectTransform;

        public RoomProductionView(GameObject view, IConfigs configs) : base(view)
        {
            _configs = configs;
            _rectTransform = view.transform as RectTransform;
        }

        protected override void UpdateViewModel(RoomProductionViewModel viewModel)
        {
            GameObject.name = $"RoomProduction_{viewModel.Type}";
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _rectTransform.pivot = new Vector2(0.5f, 0.5f);
            _rectTransform.localScale = Vector3.one;

            var canvasGroup = GameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = GameObject.AddComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            if (_configs.UI.ResourceIcons.TryGetValue(viewModel.Type, out var icon))
                _view.Icon.sprite = icon;
            _view.Icon.preserveAspect = true;
            _view.Time.gameObject.SetActive(false);

            Bind(viewModel.Production, SetProduction);
            Register(Observable.EveryLateUpdate().Subscribe(_ => UpdatePosition()));
            UpdatePosition();
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

        private void SetProduction(float production)
        {
            var rounded = Mathf.Round(production);
            _view.Value.text = Mathf.Approximately(production, rounded)
                ? Mathf.RoundToInt(production).ToString()
                : $"{production:0.#}";
        }
    }
}
