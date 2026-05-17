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
        private ISectorPlayerModel _playerModel;
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
            InitializePlayer();
            _sector.InitializePoints();
            _sectorWindow.ShowWindow();
        }

        public void Exit()
        {
            _isActive = false;
            _sectorWindow.CloseWindow();
            ReleasePlayer();
        }

        public void Update()
        {
            if (!_isActive || _playerModel == null) return;

            _playerModel.Update();
        }

        private void InitializePlayer()
        {
            ReleasePlayer();
            _playerModel = new SectorPlayerModel();
            _playerView = Object.FindAnyObjectByType<PlayerPoint>(FindObjectsInactive.Include);
            _playerModel.SetMoveSpeed(_playerView.Speed);
            _camera.SetTarget(_playerView.transform);
            _playerModel.Changed += OnPlayerChanged;
            _sector.SetPlayerModel(_playerModel);
        }

        private void ReleasePlayer()
        {
            _sector.ClearPlayerModel();
            if (_playerModel != null) _playerModel.Changed -= OnPlayerChanged;
            _camera.ClearTarget();
            _playerModel = null;
            _playerView = null;
        }

        private void OnPlayerChanged()
        {
            if (_playerModel == null || _playerView == null) return;
            _playerView.ApplyState(_playerModel.Position, _playerModel.IsMoving);
        }
    }
}
