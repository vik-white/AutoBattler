using Unity.Collections;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    public interface IBattleState : IState { }

    public interface IBattleStartState : IBattleState { }

    public class BattleStartState : IBattleStartState
    {
        private static readonly Vector3 BattleCameraPosition = new(-61.28f, 58.9f, -35.26f);

        private readonly IStateMachine<IBattleState> _stateMachine;
        private readonly IBattleWindow _battleWindow;
        private readonly ISquadWindow _squadWindow;
        private readonly ICameraService _camera;

        public BattleStartState(IStateMachine<IBattleState> stateMachine, IBattleWindow battleWindow, ISquadWindow squadWindow, ICameraService camera)
        {
            _stateMachine = stateMachine;
            _battleWindow = battleWindow;
            _squadWindow = squadWindow;
            _camera = camera;
        }

        public void Enter()
        {
            DefeatBattleEventSystem.OnExecute = _ =>_stateMachine.SwitchState<IBattleDefeatState>();
            VictoryBattleEventSystem.OnExecute = _ => _stateMachine.SwitchState<IBattleVictoryState>();

            _camera.MoveTo(BattleCameraPosition, 0.5f);
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
            if (_squadWindow.IsShowing) _squadWindow.CloseWindow();
            _battleWindow.Hide();
        }
    }
}
