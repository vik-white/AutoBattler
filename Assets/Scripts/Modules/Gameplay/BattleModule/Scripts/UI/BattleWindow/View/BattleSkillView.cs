using UniRx;
using UnityEngine;

namespace vikwhite
{
    public class BattleSkillView : View<BattleSkillHierarchy, BattleSkillViewModel>
    {
        public BattleSkillView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(BattleSkillViewModel viewModel)
        {
            _view.RarityBG.sprite = viewModel.RarityBG;
            BindClick(_view.Button, viewModel.Activate);
            viewModel.Died += OnDied;
            Register(Observable.EveryUpdate().Subscribe(_ => UpdateBars()));

            Register(Disposable.Create(() => viewModel.Died -= OnDied));

            SetDeadState(viewModel.IsDead);
            SetCharacterImage(viewModel.ImagePrefab);
            UpdateBars();
        }

        private void UpdateBars()
        {
            if (BaseViewModel == null || BaseViewModel.IsDead) return;
            var cooldown = BaseViewModel.GetCooldown();
            _view.Time.text = cooldown.ToString();
            _view.Time.gameObject.SetActive(cooldown > 0);
            _view.HealthBar.SetProgress(BaseViewModel.GetHealthProgress());
            SetProgress(_view.SkillBar, BaseViewModel.GetCooldownProgress());
        }

        private void OnDied()
        {
            SetDeadState(true);
        }

        private void SetDeadState(bool isDead)
        {
            if (_view.HealthBar != null) _view.HealthBar.gameObject.SetActive(!isDead);
            if (_view.Button != null) _view.Button.interactable = !isDead;
            _view.Time.gameObject.SetActive(!isDead);
            SetProgress(_view.SkillBar, 0);
        }
        
        private void SetCharacterImage(GameObject imagePrefab)
        {
            _view.HeroContainer.ClearChildren();
            if (imagePrefab == null) return;
            Object.Instantiate(imagePrefab, _view.HeroContainer, false);
        }

        private static void SetProgress(RectTransform progressBar, float value)
        {
            progressBar.localScale = new Vector3(1, 1 - Mathf.Clamp01(value), 1);
        }
    }
}
