using UnityEngine;

namespace vikwhite
{
    public class LobbyWindowView : WindowView<LobbyWindowHierarchy, LobbyWindowViewModel>
    {
        private readonly IResourceViewFactory _resourceViewFactory;
        
        public LobbyWindowView(GameObject view, IResourceViewFactory resourceViewFactory) : base(view)
        {
            _resourceViewFactory = resourceViewFactory;
        }
        
        protected override void UpdateViewModel(LobbyWindowViewModel viewModel)
        {
            BindClick(_view.CheatsButton, viewModel.OnCheats);
            BindClick(_view.FightButton, viewModel.OnFight);
            _view.ResourcesContainer.ClearChildren();
            _view.Location.text = viewModel.CurrentLocation;
            foreach (var resource in viewModel.Resources)
                _resourceViewFactory.Get(resource, _view.ResourcesContainer);
        }
    }
}