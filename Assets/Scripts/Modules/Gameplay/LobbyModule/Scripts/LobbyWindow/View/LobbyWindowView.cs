using UnityEngine;

namespace vikwhite
{
    public class LobbyWindowView : WindowView<LobbyWindowHierarchy, LobbyWindowViewModel>
    {
        public LobbyWindowView(GameObject view) : base(view)
        {
        }
        
        protected override void UpdateViewModel(LobbyWindowViewModel viewModel)
        {
            BindClick(_view.CheatsButton, viewModel.OnCheats);
            BindClick(_view.SquadButton, viewModel.OnSquad);
        }
    }
}