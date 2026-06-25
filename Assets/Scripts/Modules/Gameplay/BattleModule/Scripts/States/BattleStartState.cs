using Rukhanka.Toolbox;
using Unity.Collections;
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
        private static readonly Vector3 BattleCameraPosition = new(-50.18f, 48.1f, -28.88f);

        private readonly ILocationProvider _locationProvider;
        private readonly ISquadService _squad;
        private readonly IStateMachine<IBattleState> _stateMachine;
        private readonly IConfigs _configs;
        private readonly IBattleWindow _battleWindow;
        private readonly ICameraService _camera;

        public BattleStartState(ILocationProvider locationProvider, ISquadService squad, IStateMachine<IBattleState> stateMachine, IConfigs configs, IBattleWindow battleWindow, ICameraService camera)
        {
            _locationProvider = locationProvider;
            _squad = squad;
            _stateMachine = stateMachine;
            _configs = configs;
            _battleWindow = battleWindow;
            _camera = camera;
        }

        public void Enter()
        {
            TimeSystem.Reset();
            _battleWindow.Show();

            ECSWorld.SetManagedEnabled<BattleSystemGroup>(true);
            ECSWorld.SetEnabled<EndBattleSystem>(true);
            ECSWorld.SetEnabled<InitializeTimeSystem>(true);
            ECSWorld.SetEnabled<VFXConfigInitializeSystem>(true); 
            ECSWorld.SetEnabled<CharacterConfigInitializeSystem>(true);

            var initializeSquad = new InitializeSquad { Value = new FixedList512Bytes<CreateCharacter>() };
            foreach (var character in _squad.GetCharacters())
            {
                var createCharacter = character != null ? new CreateCharacter
                {
                    ID = character.ID.CalculateHash32(), 
                    Level = character.Level.Value,
                    Stars = character.Stars.Value
                } : default;
                initializeSquad.Value.Add(createCharacter);
            }
            ECSWorld.CreateEntity(initializeSquad); 

            var locationType = _configs.Map.Get(_locationProvider.ID).Type;
            if (locationType == LocationType.Static) 
                ECSWorld.CreateEntity(new InitializeStaticEnemies{ ID = _locationProvider.ID.CalculateHash32() });
            if (locationType == LocationType.Flow) 
                ECSWorld.CreateEntity(new LocationEnemiesFlow{ ID = _locationProvider.ID.CalculateHash32() });

            DefeatBattleEventSystem.OnExecute = _ =>_stateMachine.SwitchState<IBattleDefeatState>();
            VictoryBattleEventSystem.OnExecute = _ => _stateMachine.SwitchState<IBattleVictoryState>();
            
            _camera.Initialize(BattleCameraPosition, Quaternion.Euler(39.456f, 60.041f, 0.26f), 10);
        }

        public void Exit()
        {
            TimeSystem.SetPaused(false);
            _battleWindow.Hide();
        }
    }
}
