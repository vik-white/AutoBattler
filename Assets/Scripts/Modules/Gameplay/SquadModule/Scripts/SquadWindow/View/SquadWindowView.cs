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
            _view.PlayerMight.text = "0";
            _view.EnemyMight.text = "0";
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.FightButton, viewModel.StartFight);
            Bind(viewModel.CanFight, canFight => _view.FightButton.interactable = canFight);
            _view.SquadItemsContainer.ClearChildren();
            foreach (var character in viewModel.Characters)
                _squadItemViewFactory.Get(character, _view.SquadItemsContainer);
        }
    }
}
