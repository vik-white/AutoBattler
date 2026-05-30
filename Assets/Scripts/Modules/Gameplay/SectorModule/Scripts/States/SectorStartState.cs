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
            _sectorWindow.ShowWindow(_player);
            _camera.Initialize(new Vector3(-6.099998f, 7.28f, -7.16f), Quaternion.Euler(32.098f, 40.58f, 1.131f), 60, _playerView.transform);
        }

        public void Exit()
        {
            _camera.DetachFromParent();
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
