using Rukhanka.Toolbox;
using UnityEngine;
using vikwhite.Data;
using vikwhite.ECS;

namespace vikwhite
{
    public interface IBattleSquadState : IBattleState { }

    public class BattleSquadState : IBattleSquadState
    {
        private static readonly Vector3 SquadCameraPosition = new(-61.7f, 57.5f, -35.5f);

        private readonly ILocationProvider _locationProvider;
        private readonly IConfigs _configs;
        private readonly ISquadWindow _squadWindow;
        private readonly IBattleWindow _battleWindow;
        private readonly ISquadService _squad;
        private readonly IBattleSquadPlacementService _squadPlacement;
        private readonly IBattleMightService _battleMight;
        private readonly ICameraService _camera;

        public BattleSquadState(ILocationProvider locationProvider, IConfigs configs, ISquadWindow squadWindow, IBattleWindow battleWindow, ISquadService squad, IBattleSquadPlacementService squadPlacement, IBattleMightService battleMight, ICameraService camera)
        {
            _locationProvider = locationProvider;
            _configs = configs;
            _squadWindow = squadWindow;
            _battleWindow = battleWindow;
            _squad = squad;
            _squadPlacement = squadPlacement;
            _battleMight = battleMight;
            _camera = camera;
        }

        public void Enter()
        {
            _squad.Clear();
            _battleMight.UpdateEnemyMight();
            _squadPlacement.Begin();
            TimeSystem.Reset();

            ECSWorld.SetManagedEnabled<BattleSystemGroup>(true);
            BattleSystemGroup.AllowSetupWhilePaused = true;
            ECSWorld.SetEnabled<EndBattleSystem>(false);
            ECSWorld.SetEnabled<InitializeTimeSystem>(false);
            ECSWorld.SetEnabled<VFXConfigInitializeSystem>(true);
            ECSWorld.SetEnabled<CharacterConfigInitializeSystem>(true);
            TimeSystem.SetPaused(true);

            InitializeLocation();
            _camera.Initialize(SquadCameraPosition, Quaternion.Euler(39.456f, 60.041f, 0.26f), 10);
            _battleWindow.Show();
            _squadWindow.ShowWindow();
        }

        private void InitializeLocation()
        {
            var locationType = _configs.Map.Get(_locationProvider.ID).Type;
            if (locationType == LocationType.Static)
                ECSWorld.CreateEntity(new InitializeStaticEnemies { ID = _locationProvider.ID.CalculateHash32() });
            if (locationType == LocationType.Flow)
                ECSWorld.CreateEntity(new LocationEnemiesFlow { ID = _locationProvider.ID.CalculateHash32() });
        }

        public void Exit()
        {
            _squadPlacement.End();
            BattleSystemGroup.AllowSetupWhilePaused = false;
        }
    }
}
