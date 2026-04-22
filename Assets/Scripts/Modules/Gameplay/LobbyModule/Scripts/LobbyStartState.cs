using UnityEngine;
using UnityEngine.InputSystem;

namespace vikwhite
{
    public interface ILobbyState : IState { }
    
    public interface ILobbyStartState : ILobbyState { }

    public class LobbyStartState : ILobbyStartState
    {
        private readonly ICheatWindow _cheatWindow;
        
        public LobbyStartState(ICheatWindow cheatWindow)
        {
            _cheatWindow = cheatWindow;
        }
        
        public void Enter() 
        {
            _cheatWindow.ShowWindow();
        }

        public void Exit()
        {
            _cheatWindow.CloseWindow();
        }
    }
}