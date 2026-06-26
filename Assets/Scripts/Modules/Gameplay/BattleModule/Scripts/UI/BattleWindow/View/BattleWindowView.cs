using UniRx;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    public class BattleWindowView : WindowView<BattleWindowHierarchy, BattleWindowViewModel>
    {
        private static readonly Color ActiveAutoBackgroundColor = new(0.78f, 1f, 0.78f, 1f);
        private static readonly Color ActiveAutoTextColor = new(0.36f, 1f, 0.32f, 1f);

        private readonly IBattleSkillViewFactory _skillViewFactory;
        private readonly IBattleHealthBarViewFactory _healthBarViewFactory;
        private readonly IBattleDamageFlyTextViewFactory _damageFlyTextViewFactory;
        private Image _autoButtonBackground;
        private TMP_Text _autoButtonLabel;
        private Color _autoButtonBackgroundDefaultColor;
        private Color _autoButtonLabelDefaultColor;

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
            BindClick(_view.PauseButton, viewModel.OnPause);
            BindClick(_view.AutoButton, viewModel.OnToggleAutoUseSkills);
            Bind(viewModel.AutoUseSkills, SetAutoUseSkillsState);
            
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

        private void SetAutoUseSkillsState(bool enabled)
        {
            CacheAutoButtonParts();

            if (_autoButtonBackground != null)
                _autoButtonBackground.color = enabled ? ActiveAutoBackgroundColor : _autoButtonBackgroundDefaultColor;

            if (_autoButtonLabel != null)
                _autoButtonLabel.color = enabled ? ActiveAutoTextColor : _autoButtonLabelDefaultColor;
        }

        private void CacheAutoButtonParts()
        {
            if (_view.AutoButton == null || _autoButtonBackground != null || _autoButtonLabel != null) return;

            _autoButtonBackground = _view.AutoButton.GetComponentInChildren<Image>(true);
            _autoButtonLabel = _view.AutoButton.GetComponentInChildren<TMP_Text>(true);

            if (_autoButtonBackground != null) _autoButtonBackgroundDefaultColor = _autoButtonBackground.color;
            if (_autoButtonLabel != null) _autoButtonLabelDefaultColor = _autoButtonLabel.color;
        }
    }
}
