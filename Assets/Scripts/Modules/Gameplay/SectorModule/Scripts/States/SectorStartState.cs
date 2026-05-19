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

        public SectorStartState(ISectorWindow sectorWindow, ISectorService sector, ICameraService camera)
        {
            _sectorWindow = sectorWindow;
            _sector = sector;
            _camera = camera;
        }

        public void Enter()
        {
            _sector.InitializePoints();
            _player = new SectorPlayer(_sector.GetCurrentLocationPosition());
            _playerView = Object.FindAnyObjectByType<PlayerPoint>(FindObjectsInactive.Include);
            _player.OnMove += _playerView.Move;
            _player.OnStop += _playerView.Stop;
            _playerView.transform.position = _player.Position;
            _camera.SetTarget(_playerView.transform);
            _sectorWindow.ShowWindow(_player);
        }

        public void Exit()
        {
            _camera.ClearTarget();
            _sectorWindow.CloseWindow();
            _player.OnMove -= _playerView.Move;
            _player.OnStop -= _playerView.Stop;
        }

        public void Update()
        {
            _player.Update(Time.deltaTime);
        }
    }
}
