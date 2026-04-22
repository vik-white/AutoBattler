using UnityEngine;
using UnityEngine.InputSystem;

namespace vikwhite
{
    public interface ILobbyState : IState { }
    
    public interface ILobbyStartState : ILobbyState { }

    public class LobbyStartState : ILobbyStartState
    {
        private readonly ILobbyWindow _lobbyWindow;
        
        public LobbyStartState(ILobbyWindow lobbyWindow)
        {
            _lobbyWindow = lobbyWindow;
        }
        
        public void Enter() 
        {
            _lobbyWindow.ShowWindow();
        }

        public void Exit()
        {
            _lobbyWindow.CloseWindow();
        }
    }
}