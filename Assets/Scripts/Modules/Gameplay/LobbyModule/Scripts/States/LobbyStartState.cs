using UnityEngine;

namespace vikwhite
{
    public interface ILobbyState : IState { }
    
    public interface ILobbyStartState : ILobbyState { }

    public class LobbyStartState : ILobbyStartState
    {
        private readonly ILobbyWindow _lobbyWindow;
        private readonly ICameraService _camera;
        
        public LobbyStartState(ILobbyWindow lobbyWindow, ICameraService camera)
        {
            _lobbyWindow = lobbyWindow;
            _camera = camera;
        }
        
        public void Enter() 
        {
            _lobbyWindow.ShowWindow();
            _camera.Initialize(new Vector3(12.76f, 49.78f, -11.7f), Quaternion.Euler(45.97f, -45.7f, 0), 40);
            _camera.SetDragEnabled(true);
        }

        public void Exit()
        {
            _camera.SetDragEnabled(false);
            _lobbyWindow.CloseWindow();
        }
    }
}
