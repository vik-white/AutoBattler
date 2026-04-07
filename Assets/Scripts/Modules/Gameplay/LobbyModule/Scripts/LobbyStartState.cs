using UnityEngine;
using UnityEngine.InputSystem;

namespace vikwhite
{
    public interface ILobbyState : IState { }
    
    public interface ILobbyStartState : ILobbyState { }

    public class LobbyStartState : ILobbyStartState, IUpdatable
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        
        public LobbyStartState(IEnvironmentStateMachine environmentStateMachine)
        {
            _environmentStateMachine = environmentStateMachine;
        }
        
        public void Enter() 
        {
            Debug.Log("Entered Lobby");
        }

        public void Exit() { }

        public void Update()
        {
            if(Keyboard.current.bKey.wasPressedThisFrame) 
                _environmentStateMachine.SwitchState(EnvironmentType.Battle);
        }
    }
}