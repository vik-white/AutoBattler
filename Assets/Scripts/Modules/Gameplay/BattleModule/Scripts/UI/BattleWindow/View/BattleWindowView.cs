using UniRx;
using UnityEngine;

namespace vikwhite
{
    public class BattleWindowView : WindowView<BattleWindowHierarchy, BattleWindowViewModel>
    {
        private readonly IBattleSkillViewFactory _skillViewFactory;
        private readonly IBattleHealthBarViewFactory _healthBarViewFactory;
        private readonly IBattleDamageFlyTextViewFactory _damageFlyTextViewFactory;

        public BattleWindowView(GameObject view, IBattleSkillViewFactory skillViewFactory, IBattleHealthBarViewFactory healthBarViewFactory, IBattleDamageFlyTextViewFactory damageFlyTextViewFactory) : base(view)
        {
            _skillViewFactory = skillViewFactory;
            _healthBarViewFactory = healthBarViewFactory;
            _damageFlyTextViewFactory = damageFlyTextViewFactory;
        }

        protected override void UpdateViewModel(BattleWindowViewModel viewModel)
        {
            _view.SkillContainer.ClearChildren();
            _view.PlayerMight.text = viewModel.PlayerMight.ToString();
            _view.EnemyMight.text = viewModel.EnemyMight.ToString();
            
            foreach (var healthBar in viewModel.HealthBars) CreateHealthBar(healthBar);
            foreach (var skill in viewModel.Skills) CreateSkill(skill);

            viewModel.HealthBarCreated += CreateHealthBar;
            viewModel.SkillCreated += CreateSkill;
            viewModel.DamageFlyTextCreated += CreateDamageFlyText;
            Register(Disposable.Create(() => viewModel.HealthBarCreated -= CreateHealthBar));
            Register(Disposable.Create(() => viewModel.SkillCreated -= CreateSkill));
            Register(Disposable.Create(() => viewModel.DamageFlyTextCreated -= CreateDamageFlyText));
        }

        private void CreateSkill(BattleSkillViewModel skill)
        {
            _skillViewFactory.Get(skill, _view.SkillContainer);
        }

        private void CreateHealthBar(BattleHealthBarViewModel healthBar)
        {
            _healthBarViewFactory.Get(healthBar, _view.transform);
        }

        private void CreateDamageFlyText(BattleDamageFlyTextViewModel flyText)
        {
            _damageFlyTextViewFactory.Get(flyText, _view.transform);
        }
    }
}
