using System;
using UnityEngine;
using UniRx;

namespace vikwhite
{
    public class SectorWindowView : WindowView<SectorWindowHierarchy, SectorWindowViewModel>
    {
        public SectorWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(SectorWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.OnLobby);
            BindClick(_view.FightButton, viewModel.OnFight);
            Register(Observable.EveryUpdate().Subscribe(_ => Update()));
        }
        
        private void Update()
        {
            _view.Location.text = ViewModel.CurrentLocation;
            _view.FightButton.interactable = ViewModel.CanFight;
        }
    }
}
