using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    public class RoomWindowView : WindowView<RoomWindowHierarchy, RoomWindowViewModel>
    {
        private readonly IRoomLineViewFactory _roomLineFactory;
        private readonly List<RoomLineView> _roomLines = new();
        private readonly GameObject _upgradeProgressBar;
        private readonly Image _upgradeProgressFill;
        private readonly TMP_Text _upgradeProgressText;
        private bool _hasUpgrade;
        private RoomUpgradeState _upgradeState;

        public RoomWindowView(GameObject view, IRoomLineViewFactory roomLineFactory) : base(view)
        {
            _roomLineFactory = roomLineFactory;
            (_upgradeProgressBar, _upgradeProgressFill, _upgradeProgressText) = CreateUpgradeProgressBar();
        }

        protected override void UpdateViewModel(RoomWindowViewModel viewModel)
        {
            _view.ProdactionContainer.ClearChildren();
            _view.RequirementsContainer.ClearChildren();
            _view.UpgradesContainer.ClearChildren();

            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.CloseFadeButton, viewModel.Close);
            BindClick(_view.UpgradeButton, viewModel.OnUpgrade);
            Bind(viewModel.Level, level => _view.Title.text = $"{viewModel.Title} Lv.{level}");
            Bind(viewModel.CanUpgrade, canUpgrade => _view.UpgradeButton.interactable = canUpgrade);
            Bind(viewModel.HasUpgrade, SetHasUpgrade);
            Bind(viewModel.UpgradeState, SetUpgradeState);
            Bind(viewModel.Content, RefreshContent);
        }

        private (GameObject Bar, Image Fill, TMP_Text Time) CreateUpgradeProgressBar()
        {
            var bar = Object.Instantiate(
                _view.UpgradeButton.gameObject,
                _view.UpgradeButton.transform.parent);
            bar.name = "UpgradeProgress";
            bar.transform.SetSiblingIndex(_view.UpgradeButton.transform.GetSiblingIndex() + 1);

            var button = bar.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.interactable = false;
                button.enabled = false;
            }

            var background = button != null
                ? button.targetGraphic as Image
                : bar.GetComponent<Image>();
            var fillObject = new GameObject(
                "Fill",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            fillObject.layer = bar.layer;
            var fillRect = fillObject.GetComponent<RectTransform>();
            fillRect.SetParent(bar.transform, false);
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0f, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            if (background != null && background.transform.parent == bar.transform)
                fillRect.SetSiblingIndex(background.transform.GetSiblingIndex() + 1);
            else
                fillRect.SetAsFirstSibling();

            var fill = fillObject.GetComponent<Image>();
            if (background != null)
            {
                fill.sprite = background.sprite;
                fill.type = background.type;
                fill.preserveAspect = background.preserveAspect;
                fill.color = background.color;
                background.color = Color.Lerp(background.color, Color.black, 0.65f);
            }
            else
            {
                fill.color = Color.green;
            }

            foreach (var graphic in bar.GetComponentsInChildren<Graphic>(true))
                graphic.raycastTarget = false;

            var time = bar.GetComponentInChildren<TMP_Text>(true);
            if (time != null) time.transform.SetAsLastSibling();
            bar.SetActive(false);
            return (bar, fill, time);
        }

        private void SetHasUpgrade(bool hasUpgrade)
        {
            _hasUpgrade = hasUpgrade;
            RefreshUpgradeControls();
        }

        private void SetUpgradeState(RoomUpgradeState state)
        {
            _upgradeState = state;
            RefreshUpgradeControls();
        }

        private void RefreshUpgradeControls()
        {
            _view.UpgradeButton.gameObject.SetActive(_hasUpgrade && !_upgradeState.IsUpgrading);
            _upgradeProgressBar.SetActive(_upgradeState.IsUpgrading);
            if (!_upgradeState.IsUpgrading) return;

            var fillRect = _upgradeProgressFill.rectTransform;
            fillRect.anchorMax = new Vector2(_upgradeState.Progress, 1f);
            if (_upgradeProgressText != null)
                _upgradeProgressText.text = FormatRemainingTime(_upgradeState.SecondsRemaining);
        }

        private static string FormatRemainingTime(long totalSeconds)
        {
            var hours = totalSeconds / 3600;
            var minutes = totalSeconds % 3600 / 60;
            var seconds = totalSeconds % 60;
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }

        private void RefreshContent(RoomWindowContent content)
        {
            ClearRoomLines();
            AddLines(content.Production, _view.ProdactionContainer);
            AddLines(content.Requirements, _view.RequirementsContainer);
            AddLines(content.Upgrades, _view.UpgradesContainer);
        }

        private void AddLines(IReadOnlyList<RoomLineModel> lines, Transform container)
        {
            for (var i = 0; i < lines.Count; i++)
                _roomLines.Add(_roomLineFactory.Get(lines[i], container));
        }

        private void ClearRoomLines()
        {
            for (var i = 0; i < _roomLines.Count; i++)
                _roomLines[i].DisposeAndDestroy();

            _roomLines.Clear();
        }

        protected override void ReleaseViewModel()
        {
            ClearRoomLines();
            base.ReleaseViewModel();
        }
    }
}
