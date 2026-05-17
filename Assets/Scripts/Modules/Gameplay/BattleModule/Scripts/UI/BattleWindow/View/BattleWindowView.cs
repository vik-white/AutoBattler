using UniRx;
using UnityEngine;

namespace vikwhite
{
    public class BattleWindowView : WindowView<BattleWindowHierarchy, BattleWindowViewModel>
    {
        private readonly IBattleAbilityViewFactory _abilityViewFactory;
        private readonly IBattleHealthBarViewFactory _healthBarViewFactory;
        private readonly IBattleDamageFlyTextViewFactory _damageFlyTextViewFactory;

        public BattleWindowView(
            GameObject view,
            IBattleAbilityViewFactory abilityViewFactory,
            IBattleHealthBarViewFactory healthBarViewFactory,
            IBattleDamageFlyTextViewFactory damageFlyTextViewFactory) : base(view)
        {
            _abilityViewFactory = abilityViewFactory;
            _healthBarViewFactory = healthBarViewFactory;
            _damageFlyTextViewFactory = damageFlyTextViewFactory;
        }

        protected override void UpdateViewModel(BattleWindowViewModel viewModel)
        {
            BindClick(_view.LobbyButton, viewModel.OnLobby);

            foreach (var healthBar in viewModel.HealthBars)
                CreateHealthBar(healthBar);

            foreach (var ability in viewModel.Abilities)
                CreateAbility(ability);

            viewModel.HealthBarCreated += CreateHealthBar;
            viewModel.AbilityCreated += CreateAbility;
            viewModel.DamageFlyTextCreated += CreateDamageFlyText;
            Register(Observable.EveryUpdate().Subscribe(_ => UpdateHud()));

            Register(Disposable.Create(() => viewModel.HealthBarCreated -= CreateHealthBar));
            Register(Disposable.Create(() => viewModel.AbilityCreated -= CreateAbility));
            Register(Disposable.Create(() => viewModel.DamageFlyTextCreated -= CreateDamageFlyText));
        }

        private void UpdateHud()
        {
            if (_view.FPS != null && BaseViewModel != null)
                _view.FPS.text = BaseViewModel.FpsText;
        }

        private void CreateAbility(BattleAbilityViewModel ability)
        {
            if (_view.AbilityContainer == null) return;
            AddDisposable(_abilityViewFactory.Get(ability, _view.AbilityContainer));
        }

        private void CreateHealthBar(BattleHealthBarViewModel healthBar)
        {
            AddDisposable(_healthBarViewFactory.Get(healthBar, _view.transform));
        }

        private void CreateDamageFlyText(BattleDamageFlyTextViewModel flyText)
        {
            AddDisposable(_damageFlyTextViewFactory.Get(flyText, _view.transform));
        }
    }
}
