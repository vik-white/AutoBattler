using Rukhanka.Toolbox;
using UnityEngine;
using UnityEngine.InputSystem;
using vikwhite.ECS;

namespace vikwhite
{
    public interface IBattleState : IState
    {
    }

    public interface IBattleStartState : IBattleState
    {
    }

    public class BattleStartState : IBattleStartState, IUpdatable
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly ILocationProvider _locationProvider;

        public BattleStartState(IEnvironmentStateMachine environmentStateMachine, ILocationProvider locationProvider)
        {
            _environmentStateMachine = environmentStateMachine;
            _locationProvider = locationProvider;
        }

        public void Enter()
        {
            Debug.Log("Entering Battle");
            BattleHUD.Show();
            ECSWorld.Enable<InitializeVFXSystem>();
            ECSWorld.CreateEntity(new StartStaticLocation{ ID = _locationProvider.Location.CalculateHash32() });
        }

        public void Exit()
        {
            ECSWorld.DestroyScene();
            BattleHUD.Hide();
        }

        public void Update()
        {
            if (Keyboard.current.lKey.wasPressedThisFrame)
                _environmentStateMachine.SwitchState(EnvironmentType.Lobby);
        }
    }
}    
