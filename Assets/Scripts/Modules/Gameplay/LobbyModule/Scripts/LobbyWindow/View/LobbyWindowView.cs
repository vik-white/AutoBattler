using UnityEngine;

namespace vikwhite
{
    public class LobbyWindowView : WindowView<LobbyWindowHierarchy, LobbyWindowViewModel>
    {
        private readonly IEventItemViewFactory _eventItemViewFactory;
        
        public LobbyWindowView(GameObject view, IEventItemViewFactory eventItemViewFactory) : base(view)
        {
            _eventItemViewFactory = eventItemViewFactory;
        }
        
        protected override void UpdateViewModel(LobbyWindowViewModel viewModel)
        {
            BindClick(_view.AdventureButton, viewModel.OnAdventure);
            BindClick(_view.SummonButton, viewModel.OnSummon);
            BindClick(_view.MetaButton, viewModel.OnMeta);
            Bind(viewModel.Might, might => _view.Might.text = might.ToString());
            Bind(viewModel.Gems, gems => _view.Gems.text = gems.ToString());
            if (_view.EventsContainer != null)
            {
                _view.EventsContainer.ClearChildren();
                foreach (var gameEvent in viewModel.Events)
                    _eventItemViewFactory.Get(gameEvent, _view.EventsContainer);
            }
        }
    }
}
