using Rukhanka.Toolbox;
using UnityEngine;
using vikwhite.Data;
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
        private readonly ILocationProvider _locationProvider;
        private readonly ISquadService _squad;
        private readonly IStateMachine<IBattleState> _stateMachine;
        private readonly IConfigs _configs;

        public BattleStartState(ILocationProvider locationProvider, ISquadService squad, IStateMachine<IBattleState> stateMachine, IConfigs configs)
        {
            _locationProvider = locationProvider;
            _squad = squad;
            _stateMachine = stateMachine;
            _configs = configs;
        }

        public void Enter()
        {
            BattleHUD.Show();

            ECSWorld.EnableManaged<BattleSystemGroup>();
            ECSWorld.Enable<StartBattleSystem>();
            ECSWorld.Enable<InitializeTimeSystem>();
            ECSWorld.Enable<VFXConfigInitializeSystem>();
            ECSWorld.Enable<CharacterConfigInitializeSystem>();
            
            ECSWorld.CreateEntity(new InitializeSquad{ Value = _squad.GetCharactersHash() });

            var locationType = _configs.Map.Get(_locationProvider.ID).Type;
            if (locationType == LocationType.Static) 
                ECSWorld.CreateEntity(new InitializeStaticEnemies{ ID = _locationProvider.ID.CalculateHash32() });
            if (locationType == LocationType.Flow) 
                ECSWorld.CreateEntity(new LocationEnemiesFlow{ ID = _locationProvider.ID.CalculateHash32() });

            DefeatBattleEventSystem.OnExecute = _ =>_stateMachine.SwitchState<IBattleDefeatState>();
            VictoryBattleEventSystem.OnExecute = _ => _stateMachine.SwitchState<IBattleVictoryState>();
        }

        public void Exit()
        {
            ECSWorld.DisableManaged<BattleSystemGroup>();
            ECSWorld.DestroyScene();
            BattleHUD.Hide();
        }
    }
}
