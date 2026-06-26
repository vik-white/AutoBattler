using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace vikwhite
{
    public class BattleSkillView : View<BattleSkillHierarchy, BattleSkillViewModel>
    {
        private readonly Vector2 _rarityBGDefaultPosition;
        private readonly Vector2 _heroContainerDefaultPosition;
        private readonly Vector2 _glowDefaultPosition;
        private bool _animated;
        
        public BattleSkillView(GameObject view) : base(view)
        {
            _rarityBGDefaultPosition = _view.RarityBG.rectTransform.anchoredPosition;
            _heroContainerDefaultPosition = _view.HeroContainer.anchoredPosition;
            _glowDefaultPosition = _view.Glow.rectTransform.anchoredPosition;
        }

        protected override void UpdateViewModel(BattleSkillViewModel viewModel)
        {
            _view.RarityBG.sprite = viewModel.RarityBG;
            _view.RarityFrame.sprite = viewModel.RarityFrame;
            BindClick(_view.Button, viewModel.Activate);
            viewModel.OnActivate += PlayAnimation;
            viewModel.Died += OnDied;
            Register(Observable.EveryUpdate().Subscribe(_ => UpdateBars()));

            Register(Disposable.Create(() =>
            {
                viewModel.OnActivate -= PlayAnimation;
                viewModel.Died -= OnDied;
            }));

            SetDeadState(viewModel.IsDead);
            SetCharacterImage(viewModel.ImagePrefab);
            UpdateBars();
        }

        private void UpdateBars()
        {
            if (BaseViewModel == null) return;
            if (!BaseViewModel.Exists())
            {
                DisposeAndDestroy();
                return;
            }
            if (BaseViewModel.IsDead) return;

            var cooldown = BaseViewModel.GetCooldown();
            _view.Time.text = cooldown.ToString();
            _view.Lock.gameObject.SetActive(!_animated);
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
        
        private void PlayAnimation()
        {
            _animated = true;
            DOTween.Sequence()
                .SetUpdate(true)
                .Join(TweenHendler.CreateAnchoredPositionYTween(_view.RarityBG.rectTransform, _rarityBGDefaultPosition.y + 109, 0.2f).SetEase(Ease.OutCubic))
                .Join(TweenHendler.CreateAnchoredPositionYTween(_view.Glow.rectTransform, _glowDefaultPosition.y + 109, 0.2f).SetEase(Ease.OutCubic))
                .Join(TweenHendler.CreateGraphicAlphaTween(_view.Glow, 1, 0.2f).SetEase(Ease.OutCubic))
                .Join(TweenHendler.CreateAnchoredPositionYTween(_view.HeroContainer, _heroContainerDefaultPosition.y + 80, 0.2f).SetEase(Ease.OutCubic))
                .AppendInterval(1.5f)
                .Append(TweenHendler.CreateAnchoredPositionYTween(_view.RarityBG.rectTransform, _rarityBGDefaultPosition.y, 0.2f).SetEase(Ease.OutCubic))
                .Join(TweenHendler.CreateAnchoredPositionYTween(_view.Glow.rectTransform, _glowDefaultPosition.y, 0.2f).SetEase(Ease.OutCubic))
                .Join(TweenHendler.CreateGraphicAlphaTween(_view.Glow, 0, 0.2f).SetEase(Ease.OutCubic))
                .Join(TweenHendler.CreateAnchoredPositionYTween(_view.HeroContainer, _heroContainerDefaultPosition.y, 0.2f).SetEase(Ease.OutCubic))
                .AppendCallback(() => { _animated = false; });
        }

        private static void SetProgress(Image progressBar, float value)
        {
            progressBar.fillAmount = 1 - Mathf.Clamp01(value);
        }
    }
}
