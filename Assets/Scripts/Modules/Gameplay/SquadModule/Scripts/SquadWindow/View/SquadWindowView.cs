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
            Bind(viewModel.PlayerMight, might => _view.PlayerMight.text = might.ToString());
            Bind(viewModel.EnemyMight, might => _view.EnemyMight.text = might.ToString());
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.FightButton, viewModel.StartFight);
            Bind(viewModel.CanFight, canFight => _view.FightButton.interactable = canFight);
            _view.SquadItemsContainer.ClearChildren();
            foreach (var character in viewModel.Characters)
                _squadItemViewFactory.Get(character, _view.SquadItemsContainer);
        }
    }
}
