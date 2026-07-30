using UnityEngine;

namespace vikwhite
{
    public class BreakthroughWindowView :
        WindowView<BreakthroughHierarchy, BreakthroughWindowViewModel>
    {
        public BreakthroughWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(BreakthroughWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.CloseFadeButton, viewModel.Close);
            BindClick(_view.BreakthroughButton, viewModel.OnBreakthrough);
            Bind(viewModel.CurrentLevel, value => _view.CurrentLevel.text = value);
            Bind(viewModel.NextLevel, value => _view.NextLevel.text = value);
            Bind(viewModel.AttackCurrent, value => _view.AttackCurrent.text = value);
            Bind(viewModel.AttackAdd, value => _view.AttackAdd.text = value);
            Bind(viewModel.DefenseCurrent, value => _view.DefenseCurrent.text = value);
            Bind(viewModel.DefenseAdd, value => _view.DefenseAdd.text = value);
            Bind(viewModel.HealthCurrent, value => _view.HealthCurrent.text = value);
            Bind(viewModel.HealthAdd, value => _view.HealthAdd.text = value);
            Bind(viewModel.EssenceCount, value => _view.EssenceCount.text = value);
            Bind(viewModel.ExpCount, value => _view.ExpCount.text = value);
            Bind(viewModel.HeroesDesc, value => _view.HeroesDesc.text = value);
            Bind(viewModel.HeroesCount, value => _view.HeroesCount.text = value);
            Bind(viewModel.CanBreakthrough, value => _view.BreakthroughButton.interactable = value);
        }
    }
}
