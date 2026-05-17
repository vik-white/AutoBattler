using UnityEngine;

namespace vikwhite
{
    public interface ISectorState : IState { }

    public interface ISectorStartState : ISectorState
    {
        bool IsMoving { get; }
        event System.Action Changed;
        void MoveToNextLocation();
    }

    public class SectorStartState : ISectorStartState, IUpdatable
    {
        private readonly ISectorWindow _sectorWindow;
        private readonly ISectorService _sector;
        private readonly ICameraService _camera;
        private ISectorPlayerModel _playerModel;
        private PlayerPoint _playerView;
        private string _movingLocation;
        private bool _isActive;

        public bool IsMoving => _playerModel != null && _playerModel.IsMoving;
        public event System.Action Changed;

        public SectorStartState(ISectorWindow sectorWindow, ISectorService sector, ICameraService camera)
        {
            _sectorWindow = sectorWindow;
            _sector = sector;
            _camera = camera;
        }

        public void Enter()
        {
            _isActive = true;
            _sector.Changed += OnSectorChanged;
            InitializePlayer();
            _sector.InitializePoints();
            PlacePlayerAtCurrentLocation();
            _sectorWindow.ShowWindow();
        }

        public void Exit()
        {
            _isActive = false;
            _sectorWindow.CloseWindow();
            ReleasePlayer();
            _sector.Changed -= OnSectorChanged;
        }

        public void Update()
        {
            if (!_isActive || _playerModel == null) return;
            if (!_playerModel.IsMoving) return;

            var completed = _playerModel.Update(Time.deltaTime);
            ApplyPlayerView();

            if (completed) CompleteMove();
        }

        private void InitializePlayer()
        {
            ReleasePlayer();
            _playerModel = new SectorPlayerModel();
            _playerView = Object.FindAnyObjectByType<PlayerPoint>(FindObjectsInactive.Include);
            if (_playerView == null)
            {
                Debug.LogWarning("PlayerPoint was not found on sector scene.");
                return;
            }

            _playerModel.SetMoveSpeed(_playerView.Speed);
            _camera.SetTarget(_playerView.transform);
        }

        private void ReleasePlayer()
        {
            _camera.ClearTarget();
            _playerModel = null;
            _playerView = null;
            _movingLocation = string.Empty;
        }

        public void MoveToNextLocation()
        {
            if (_playerModel == null || _playerModel.IsMoving) return;
            if (!_sector.TryGetNextLocation(out var locationID, out var position)) return;

            _movingLocation = locationID;
            var completed = _playerModel.MoveTo(position);
            ApplyPlayerView();

            if (completed)
                CompleteMove();
            else
                Changed?.Invoke();
        }

        private void PlacePlayerAtCurrentLocation()
        {
            if (_playerModel == null) return;
            if (!_sector.TryGetCurrentLocationPosition(out var position)) return;

            _playerModel.PlaceAt(position);
            ApplyPlayerView();
        }

        private void CompleteMove()
        {
            if (string.IsNullOrEmpty(_movingLocation)) return;

            var completedLocation = _movingLocation;
            _movingLocation = string.Empty;
            _sector.SetCurrentLocation(completedLocation);
        }

        private void ApplyPlayerView()
        {
            if (_playerModel == null || _playerView == null) return;
            _playerView.ApplyState(_playerModel.Position, _playerModel.IsMoving);
        }

        private void OnSectorChanged()
        {
            Changed?.Invoke();
        }
    }
}
