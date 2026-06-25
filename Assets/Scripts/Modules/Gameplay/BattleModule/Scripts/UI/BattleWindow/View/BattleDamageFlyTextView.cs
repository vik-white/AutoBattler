using TMPro;
using UniRx;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    public class BattleDamageFlyTextView : View<BattleDamageFlyTextHierarchy, BattleDamageFlyTextViewModel>
    {
        private const float FlyHeight = 0.8f;
        private const float CritScaleMultiplier = 1.5f;
        private const float SpreadAngleDegrees = 25f;

        private static readonly Color EnemyDamageColor = new(1f, 0.2f, 0.2f, 1f);
        private static readonly Color SquadDamageColor = new(0.25f, 1f, 0.35f, 1f);
        private static readonly Color CritDamageColor = new(1f, 0.85f, 0.1f, 1f);

        private Camera _camera;
        private Vector3 _startPosition;
        private Vector3 _flyDirection;
        private Vector3 _baseScale;

        public BattleDamageFlyTextView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(BattleDamageFlyTextViewModel viewModel)
        {
            _camera = Camera.main;
            _startPosition = viewModel.Position + new Vector3(0, 0.5f, 0);
            _flyDirection = ComputeFlyDirection();
            _baseScale = _view.transform.localScale;

            if (viewModel.IsCrit) _view.transform.localScale = _baseScale * CritScaleMultiplier;

            if (_view.Text != null)
            {
                _view.Text.text = viewModel.Text;
                _view.Text.color = viewModel.IsCrit
                    ? CritDamageColor
                    : (viewModel.IsEnemyTarget ? EnemyDamageColor : SquadDamageColor);
                _view.Text.fontStyle = viewModel.IsCrit ? FontStyles.Bold : FontStyles.Normal;
            }

            Register(Observable.EveryUpdate().Subscribe(_ => UpdateFlyText()));

            UpdatePosition(0);
        }

        private void UpdateFlyText()
        {
            if (BaseViewModel == null) return;

            BaseViewModel.Tick(TimeSystem.DeltaTime);
            UpdatePosition(BaseViewModel.Progress);
            UpdateAlpha(BaseViewModel.Progress);

            if (BaseViewModel.IsComplete)
                DisposeAndDestroy();
        }

        private void UpdatePosition(float progress)
        {
            if (_camera == null) _camera = Camera.main;
            if (_camera == null) return;

            _view.transform.position = _camera.WorldToScreenPoint(_startPosition + _flyDirection * (FlyHeight * progress));
        }

        private Vector3 ComputeFlyDirection()
        {
            var up = _camera != null ? _camera.transform.up : Vector3.up;
            var right = _camera != null ? _camera.transform.right : Vector3.right;
            var angleRad = UnityEngine.Random.Range(-SpreadAngleDegrees, SpreadAngleDegrees) * Mathf.Deg2Rad;
            return (up * Mathf.Cos(angleRad) + right * Mathf.Sin(angleRad)).normalized;
        }

        private void UpdateAlpha(float progress)
        {
            if (_view.Text == null) return;

            var color = _view.Text.color;
            color.a = 1 - Mathf.Clamp01((progress - 0.6f) / 0.4f);
            _view.Text.color = color;
        }
    }
}
