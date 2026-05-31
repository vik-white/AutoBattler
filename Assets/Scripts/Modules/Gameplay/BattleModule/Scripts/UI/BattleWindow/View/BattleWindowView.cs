using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace vikwhite
{
    public class BattleWindowView : WindowView<BattleWindowHierarchy, BattleWindowViewModel>
    {
        private readonly IBattleSkillViewFactory _skillViewFactory;
        private readonly IBattleHealthBarViewFactory _healthBarViewFactory;
        private readonly IBattleDamageFlyTextViewFactory _damageFlyTextViewFactory;
        private readonly List<IView> _createdViews = new();

        public BattleWindowView(GameObject view, IBattleSkillViewFactory skillViewFactory, IBattleHealthBarViewFactory healthBarViewFactory, IBattleDamageFlyTextViewFactory damageFlyTextViewFactory) : base(view)
        {
            _skillViewFactory = skillViewFactory;
            _healthBarViewFactory = healthBarViewFactory;
            _damageFlyTextViewFactory = damageFlyTextViewFactory;
        }

        protected override void UpdateViewModel(BattleWindowViewModel viewModel)
        {
            BindClick(_view.QuickVictoryButton, viewModel.OnQuickVictory);

            foreach (var healthBar in viewModel.HealthBars) CreateHealthBar(healthBar);
            foreach (var skill in viewModel.Skills) CreateSkill(skill);

            viewModel.HealthBarCreated += CreateHealthBar;
            viewModel.SkillCreated += CreateSkill;
            viewModel.DamageFlyTextCreated += CreateDamageFlyText;
            Register(Observable.EveryUpdate().Subscribe(_ => UpdateFPS()));
            Register(Disposable.Create(() => viewModel.HealthBarCreated -= CreateHealthBar));
            Register(Disposable.Create(() => viewModel.SkillCreated -= CreateSkill));
            Register(Disposable.Create(() => viewModel.DamageFlyTextCreated -= CreateDamageFlyText));
        }

        protected override void ReleaseViewModel()
        {
            DestroyCreatedViews();
            base.ReleaseViewModel();
        }

        private void UpdateFPS()
        {
            if (_view.FPS != null && BaseViewModel != null) _view.FPS.text = BaseViewModel.FpsText;
        }

        private void CreateSkill(BattleSkillViewModel skill)
        {
            AddCreatedView(_skillViewFactory.Get(skill, _view.SkillContainer));
        }

        private void CreateHealthBar(BattleHealthBarViewModel healthBar)
        {
            AddCreatedView(_healthBarViewFactory.Get(healthBar, _view.transform));
        }

        private void CreateDamageFlyText(BattleDamageFlyTextViewModel flyText)
        {
            AddCreatedView(_damageFlyTextViewFactory.Get(flyText, _view.transform));
        }

        private void AddCreatedView(IView view)
        {
            _createdViews.Add(view);
        }

        private void DestroyCreatedViews()
        {
            foreach (var view in _createdViews)
            {
                if (view?.GameObject == null) continue;
                view.DisposeAndDestroy();
            }

            _createdViews.Clear();
        }
    }
}
