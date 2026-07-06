using UnityEngine;
using UnityEngine.InputSystem;

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
            _camera.Initialize(new Vector3(-61.28f, 58.9f, -35.26f), Quaternion.Euler(39.456f, 60.041f, 0.26f), 10);
        }

        public void Exit()
        {
            _lobbyWindow.CloseWindow();
        }
    }
}