using Unity.Collections;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    public interface IBattleState : IState
    {
    }

    public interface IBattleStartState : IBattleState
    {
    }

    public class BattleStartState : IBattleStartState
    {
        private static readonly Vector3 BattleCameraPosition = new(-61.28f, 58.9f, -35.26f);

        private readonly IStateMachine<IBattleState> _stateMachine;
        private readonly IBattleWindow _battleWindow;
        private readonly ICameraService _camera;

        public BattleStartState(IStateMachine<IBattleState> stateMachine, IBattleWindow battleWindow, ICameraService camera)
        {
            _stateMachine = stateMachine;
            _battleWindow = battleWindow;
            _camera = camera;
        }

        public void Enter()
        {
            DefeatBattleEventSystem.OnExecute = _ =>_stateMachine.SwitchState<IBattleDefeatState>();
            VictoryBattleEventSystem.OnExecute = _ => _stateMachine.SwitchState<IBattleVictoryState>();

            _camera.Initialize(BattleCameraPosition, Quaternion.Euler(39.456f, 60.041f, 0.26f), 10);
            _battleWindow.Show();
            ECSWorld.CreateEntity(new InitializeSquad { Value = new FixedList512Bytes<CreateCharacter>() });
            ECSWorld.SetEnabled<EndBattleSystem>(true);
            BattleSystemGroup.AllowSetupWhilePaused = false;
            TimeSystem.SetPaused(false);
        }

        public void Exit()
        {
            DefeatBattleEventSystem.OnExecute = null;
            VictoryBattleEventSystem.OnExecute = null;
            TimeSystem.SetPaused(false);
            _battleWindow.Hide();
        }
    }
}
