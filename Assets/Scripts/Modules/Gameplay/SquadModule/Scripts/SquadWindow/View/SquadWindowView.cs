using UnityEngine;

namespace vikwhite
{
    public class SquadWindowView : WindowView<SquadWindowHierarchy, SquadWindowViewModel>
    {
        private readonly ISquadItemViewFactory _squadItemViewFactory;
        
        public SquadWindowView(GameObject view, ISquadItemViewFactory squadItemViewFactory) : base(view)
        {
            _squadItemViewFactory = squadItemViewFactory;
        }
        
        protected override void UpdateViewModel(SquadWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.FightButton, viewModel.StartFight);
            _view.SquadItemsContainer.ClearChildren();
            foreach (var character in viewModel.Characters)
                _squadItemViewFactory.Get(character, _view.SquadItemsContainer);
        }
    }
}