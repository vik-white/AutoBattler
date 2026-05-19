using UnityEngine;

namespace vikwhite
{
    public interface ISectorState : IState { }

    public interface ISectorStartState : ISectorState { }

    public class SectorStartState : ISectorStartState, IUpdatable
    {
        private readonly ISectorWindow _sectorWindow;
        private readonly ISectorService _sector;
        private readonly ICameraService _camera;
        private SectorPlayer _player;
        private PlayerPoint _playerView;
        private bool _isActive;

        public SectorStartState(ISectorWindow sectorWindow, ISectorService sector, ICameraService camera)
        {
            _sectorWindow = sectorWindow;
            _sector = sector;
            _camera = camera;
        }

        public void Enter()
        {
            _isActive = true;
            _player = new SectorPlayer(_sector);
            _player.Changed += ApplyPlayerView;
            InitializePlayer();
            _sector.InitializePoints();
            PlacePlayerAtCurrentLocation();
            _sectorWindow.ShowWindow(_player);
        }

        public void Exit()
        {
            _isActive = false;
            _sectorWindow.CloseWindow();
            ReleasePlayer();
            _player.Changed -= ApplyPlayerView;
        }

        public void Update()
        {
            if (!_isActive) return;
            if (!_player.IsMoving) return;

            _player.Update(Time.deltaTime);
            ApplyPlayerView();
        }

        private void InitializePlayer()
        {
            ReleasePlayer();
            _playerView = Object.FindAnyObjectByType<PlayerPoint>(FindObjectsInactive.Include);
            if (_playerView == null)
            {
                Debug.LogWarning("PlayerPoint was not found on sector scene.");
                return;
            }

            _player.SetMoveSpeed(_playerView.Speed);
            _camera.SetTarget(_playerView.transform);
        }

        private void ReleasePlayer()
        {
            _camera.ClearTarget();
            _playerView = null;
        }

        private void PlacePlayerAtCurrentLocation()
        {
            if (!_sector.TryGetCurrentLocationPosition(out var position)) return;

            _player.PlaceAt(position);
            ApplyPlayerView();
        }

        private void ApplyPlayerView()
        {
            if (_playerView == null) return;
            _playerView.ApplyState(_player.Position, _player.IsMoving);
        }
    }
}
