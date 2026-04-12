using UnityEngine;
using UnityEngine.InputSystem;

namespace vikwhite
{
    public interface ILobbyState : IState { }
    
    public interface ILobbyStartState : ILobbyState { }

    public class LobbyStartState : ILobbyStartState
    {
        private readonly IMapWindow _mapWindow;
        
        public LobbyStartState(IMapWindow mapWindow)
        {
            _mapWindow = mapWindow;
        }
        
        public void Enter() 
        {
            _mapWindow.ShowWindow();
        }

        public void Exit()
        {
            _mapWindow.CloseWindow();
        }
    }
}