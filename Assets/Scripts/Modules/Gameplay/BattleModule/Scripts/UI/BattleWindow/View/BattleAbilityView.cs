using UniRx;
using UnityEngine;

namespace vikwhite
{
    public class BattleAbilityView : View<BattleAbilityHierarchy, BattleAbilityViewModel>
    {
        public BattleAbilityView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(BattleAbilityViewModel viewModel)
        {
            if (_view.Icon != null) _view.Icon.sprite = viewModel.Icon;
            if (_view.Fade != null) _view.Fade.sprite = viewModel.Icon;
            if (_view.Title != null) _view.Title.text = viewModel.Title;

            BindClick(_view.Button, viewModel.Activate);
            viewModel.Died += OnDied;
            _view.Updated += UpdateBars;

            Register(Disposable.Create(() => viewModel.Died -= OnDied));
            Register(Disposable.Create(() => _view.Updated -= UpdateBars));

            SetDeadState(viewModel.IsDead);
            UpdateBars();
        }

        private void UpdateBars()
        {
            if (BaseViewModel == null || BaseViewModel.IsDead) return;

            SetProgress(_view.HealthBar, BaseViewModel.GetHealthProgress());
            SetProgress(_view.AbilityBar, BaseViewModel.GetCooldownProgress());
        }

        private void OnDied()
        {
            SetDeadState(true);
        }

        private void SetDeadState(bool isDead)
        {
            if (_view.Fade != null) _view.Fade.gameObject.SetActive(isDead);
            if (_view.HealthBar != null) _view.HealthBar.gameObject.SetActive(!isDead);
            if (_view.AbilityBar != null) _view.AbilityBar.gameObject.SetActive(!isDead);
            if (_view.Button != null) _view.Button.interactable = !isDead;
        }

        private static void SetProgress(RectTransform progressBar, float value)
        {
            if (progressBar == null) return;
            progressBar.localScale = new Vector3(Mathf.Clamp01(value), 1, 1);
        }
    }
}
