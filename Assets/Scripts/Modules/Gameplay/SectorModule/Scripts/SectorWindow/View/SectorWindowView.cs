using System;
using UnityEngine;

namespace vikwhite
{
    public class SectorWindowView : WindowView<SectorWindowHierarchy, SectorWindowViewModel>
    {
        public SectorWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(SectorWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.OnLobby);
            BindClick(_view.FightButton, viewModel.OnFight);
            BindClick(_view.GoToNextButton, viewModel.OnGoToNext);
            viewModel.Changed += Refresh;
            Refresh();
        }
        
        private void Refresh()
        {
            if (_view.Location != null) _view.Location.text = ViewModel.CurrentLocation;
            if (_view.GoToNextButton != null) _view.GoToNextButton.interactable = ViewModel.CanGoToNext;
            if (_view.FightButton != null) _view.FightButton.interactable = ViewModel.CanFight;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            ViewModel.Changed -= Refresh;
        }
    }
}
