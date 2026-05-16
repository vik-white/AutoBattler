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
            _view.Location.text = viewModel.CurrentLocation;
        }
    }
}
