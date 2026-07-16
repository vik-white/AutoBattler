using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace vikwhite
{
    public class RoomWindowView : WindowView<RoomWindowHierarchy, RoomWindowViewModel>
    {
        private readonly IRoomLineViewFactory _roomLineFactory;
        private readonly List<RoomLineView> _roomLines = new();
        private bool _hasUpgrade;
        private RoomUpgradeState _upgradeState;

        public RoomWindowView(GameObject view, IRoomLineViewFactory roomLineFactory) : base(view)
        {
            _roomLineFactory = roomLineFactory;
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
            _view.ProgressBar.gameObject.SetActive(_upgradeState.IsUpgrading);
            if (!_upgradeState.IsUpgrading) return;

            _view.ProgressBar.SetProgress(_upgradeState.Progress);
            _view.ProgressBar.SetText(FormatRemainingTime(_upgradeState.SecondsRemaining));
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
