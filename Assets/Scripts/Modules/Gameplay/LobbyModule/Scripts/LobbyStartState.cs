using UnityEngine;
using UnityEngine.InputSystem;

namespace vikwhite
{
    public interface ILobbyState : IState { }
    
    public interface ILobbyStartState : ILobbyState { }

    public class LobbyStartState : ILobbyStartState, IUpdatable
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly IMapWindow _mapWindow;
        
        public LobbyStartState(IEnvironmentStateMachine environmentStateMachine, IMapWindow mapWindow)
        {
            _environmentStateMachine = environmentStateMachine;
            _mapWindow = mapWindow;
        }
        
        public void Enter() 
        {
            Debug.Log("Entered Lobby");
            _mapWindow.ShowWindow();
        }

        public void Exit()
        {
            _mapWindow.CloseWindow();
        }

        public void Update()
        {
            if(Keyboard.current.bKey.wasPressedThisFrame) 
                _environmentStateMachine.SwitchState(EnvironmentType.Battle);
        }
    }
}