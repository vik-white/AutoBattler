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
        private static readonly Vector3 SquadCameraPosition = new(-51.07f, 47.36f, -29.39f);

        private readonly ILocationProvider _locationProvider;
        private readonly IStateMachine<IBattleState> _stateMachine;
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly IConfigs _configs;
        private readonly IBattleWindow _battleWindow;
        private readonly ISquadWindow _squadWindow;
        private readonly ISquadService _squad;
        private readonly IBattleSquadPlacementService _squadPlacement;
        private readonly ICameraService _camera;

        public BattleStartState(
            ILocationProvider locationProvider,
            IStateMachine<IBattleState> stateMachine,
            IEnvironmentStateMachine environmentStateMachine,
            IConfigs configs,
            IBattleWindow battleWindow,
            ISquadWindow squadWindow,
            ISquadService squad,
            IBattleSquadPlacementService squadPlacement,
            ICameraService camera)
        {
            _locationProvider = locationProvider;
            _stateMachine = stateMachine;
            _environmentStateMachine = environmentStateMachine;
            _configs = configs;
            _battleWindow = battleWindow;
            _squadWindow = squadWindow;
            _squad = squad;
            _squadPlacement = squadPlacement;
            _camera = camera;
        }

        public void Enter()
        {
            _squad.Clear();
            _squadPlacement.Begin();
            _squad.FightRequested += StartFight;
            _squad.BackRequested += ReturnToMap;
            TimeSystem.Reset();

            ECSWorld.SetManagedEnabled<BattleSystemGroup>(true);
            BattleSystemGroup.AllowSetupWhilePaused = true;
            ECSWorld.SetEnabled<EndBattleSystem>(false);
            ECSWorld.SetEnabled<InitializeTimeSystem>(false);
            ECSWorld.SetEnabled<VFXConfigInitializeSystem>(true); 
            ECSWorld.SetEnabled<CharacterConfigInitializeSystem>(true);
            TimeSystem.SetPaused(true);

            var locationType = _configs.Map.Get(_locationProvider.ID).Type;
            if (locationType == LocationType.Static) 
                ECSWorld.CreateEntity(new InitializeStaticEnemies{ ID = _locationProvider.ID.CalculateHash32() });
            if (locationType == LocationType.Flow) 
                ECSWorld.CreateEntity(new LocationEnemiesFlow{ ID = _locationProvider.ID.CalculateHash32() });

            DefeatBattleEventSystem.OnExecute = _ =>_stateMachine.SwitchState<IBattleDefeatState>();
            VictoryBattleEventSystem.OnExecute = _ => _stateMachine.SwitchState<IBattleVictoryState>();
            
            _camera.Initialize(SquadCameraPosition, Quaternion.Euler(39.456f, 60.041f, 0.26f), 10);
            _squadWindow.ShowWindow();
        }

        public void Exit()
        {
            _squadPlacement.End();
            _squad.FightRequested -= StartFight;
            _squad.BackRequested -= ReturnToMap;
            BattleSystemGroup.AllowSetupWhilePaused = false;
            TimeSystem.SetPaused(false);
            if (_squadWindow.IsShowing) _squadWindow.CloseWindow();
            _battleWindow.Hide();
        }

        private void StartFight()
        {
            _camera.Initialize(BattleCameraPosition, Quaternion.Euler(39.456f, 60.041f, 0.26f), 10);
            _squadPlacement.End();
            _battleWindow.Show();
            ECSWorld.CreateEntity(new InitializeSquad
            {
                Value = new FixedList512Bytes<CreateCharacter>()
            });
            ECSWorld.SetEnabled<EndBattleSystem>(true);
            BattleSystemGroup.AllowSetupWhilePaused = false;
            TimeSystem.SetPaused(false);
        }

        private void ReturnToMap()
        {
            _environmentStateMachine.SwitchState(EnvironmentType.Sector);
        }
    }
}
