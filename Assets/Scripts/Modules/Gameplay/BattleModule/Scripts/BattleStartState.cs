using UnityEngine;
using UnityEngine.InputSystem;

namespace vikwhite
{
    public interface IBattleState : IState { }
    
    public interface IBattleStartState : IBattleState { }
    
    public class BattleStartState : IBattleStartState, IUpdatable
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        
        public BattleStartState(IEnvironmentStateMachine environmentStateMachine)
        {
            _environmentStateMachine = environmentStateMachine;
        }

        public void Enter() 
        {
            Debug.Log("Entering Battle");
        }

        public void Exit() { }
        
        public void Update()
        {
            if(Keyboard.current.lKey.wasPressedThisFrame) 
                _environmentStateMachine.SwitchState(EnvironmentType.Lobby);
        }
    }
}