using UnityEngine;

namespace vikwhite
{
    public class LobbyWindowView : WindowView<LobbyWindowHierarchy, LobbyWindowViewModel>
    {
        private readonly IResourceViewFactory _resourceViewFactory;
        private readonly IEventItemViewFactory _eventItemViewFactory;
        
        public LobbyWindowView(GameObject view, IResourceViewFactory resourceViewFactory, IEventItemViewFactory eventItemViewFactory) : base(view)
        {
            _resourceViewFactory = resourceViewFactory;
            _eventItemViewFactory = eventItemViewFactory;
        }
        
        protected override void UpdateViewModel(LobbyWindowViewModel viewModel)
        {
            BindClick(_view.CheatsButton, viewModel.OnCheats);
            BindClick(_view.MapButton, viewModel.OnMap);
            BindClick(_view.BankButton, viewModel.OnBank);
            _view.ResourcesContainer.ClearChildren();
            foreach (var resource in viewModel.Resources)
                _resourceViewFactory.Get(resource, _view.ResourcesContainer);

            if (_view.EventsContainer != null)
            {
                _view.EventsContainer.ClearChildren();
                foreach (var gameEvent in viewModel.Events)
                    _eventItemViewFactory.Get(gameEvent, _view.EventsContainer);
            }
        }
    }
}
