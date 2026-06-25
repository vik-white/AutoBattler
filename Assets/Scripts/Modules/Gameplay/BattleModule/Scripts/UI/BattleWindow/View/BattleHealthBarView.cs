using UniRx;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    public class BattleHealthBarView : View<BattleHealthBarHierarchy, BattleHealthBarViewModel>
    {
        private const float WhiteHealthDelay = 0.5f;
        private const float WhiteHealthDecreaseSpeed = 1.5f;

        private bool _isHealthInitialized;
        private bool _isWhiteHealthDecreasing;
        private float _healthFill;
        private float _whiteHealthFill;
        private float _whiteHealthDelayLeft;

        public BattleHealthBarView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(BattleHealthBarViewModel viewModel)
        {
            if (_view.HealthProgressBarImage != null)
                _view.HealthProgressBarImage.color = viewModel.IsEnemy ? _view.EnemyColor : _view.SquadColor;

            viewModel.Died += OnDied;
            Register(Observable.EveryUpdate().Subscribe(_ => UpdateBar()));

            Register(Disposable.Create(() => viewModel.Died -= OnDied));

            UpdateBar();
        }

        private void UpdateBar()
        {
            if (BaseViewModel == null) return;
            if (!BaseViewModel.Exists())
            {
                DisposeAndDestroy();
                return;
            }

            var camera = Camera.main;
            if (camera != null)
                _view.transform.position = camera.WorldToScreenPoint(BaseViewModel.GetHeadPosition());

            UpdateHealthBars(BaseViewModel.GetHealthFill());

            var isShowShield = BaseViewModel.IsShieldVisible();
            if (_view.ShieldBar != null) _view.ShieldBar.SetActive(isShowShield);
            if (isShowShield) SetProgressBarScale(_view.ShieldProgressBar, BaseViewModel.GetShieldFill());
        }

        private void OnDied()
        {
            DisposeAndDestroy();
        }

        private void UpdateHealthBars(float healthFill)
        {
            if (!_isHealthInitialized)
            {
                _healthFill = healthFill;
                _whiteHealthFill = healthFill;
                _isHealthInitialized = true;
                SetProgressBarScale(_view.HealthProgressBar, _healthFill);
                SetProgressBarScale(_view.HealthWhiteProgressBar, _whiteHealthFill);
                return;
            }

            var previousHealthFill = _healthFill;
            _healthFill = healthFill;
            SetProgressBarScale(_view.HealthProgressBar, _healthFill);

            if (_healthFill >= _whiteHealthFill)
            {
                _whiteHealthDelayLeft = 0;
                _isWhiteHealthDecreasing = false;
                _whiteHealthFill = _healthFill;
                SetProgressBarScale(_view.HealthWhiteProgressBar, _whiteHealthFill);
                return;
            }

            if (_healthFill < previousHealthFill && !_isWhiteHealthDecreasing && _whiteHealthDelayLeft <= 0)
                _whiteHealthDelayLeft = WhiteHealthDelay;

            if (_whiteHealthDelayLeft > 0)
            {
                _whiteHealthDelayLeft -= TimeSystem.DeltaTime;
                SetProgressBarScale(_view.HealthWhiteProgressBar, _whiteHealthFill);
                return;
            }

            _isWhiteHealthDecreasing = true;
            _whiteHealthFill = Mathf.MoveTowards(_whiteHealthFill, _healthFill, WhiteHealthDecreaseSpeed * TimeSystem.DeltaTime);
            if (Mathf.Approximately(_whiteHealthFill, _healthFill))
                _isWhiteHealthDecreasing = false;

            SetProgressBarScale(_view.HealthWhiteProgressBar, _whiteHealthFill);
        }

        private static void SetProgressBarScale(RectTransform progressBar, float value)
        {
            if (progressBar == null) return;
            progressBar.localScale = new Vector3(Mathf.Clamp01(value), 1, 1);
        }
    }
}
