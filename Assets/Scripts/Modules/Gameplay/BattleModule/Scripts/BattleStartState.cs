using Rukhanka.Toolbox;
using UnityEngine;
using UnityEngine.InputSystem;
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

        public BattleStartState(ILocationProvider locationProvider)
        {
            _locationProvider = locationProvider;
        }

        public void Enter()
        {
            BattleHUD.Show();
            ECSWorld.Enable<InitializeVFXSystem>();
            ECSWorld.CreateEntity(new InitializeSquad());
            if(_locationProvider.Type == LocationType.Static) 
                ECSWorld.CreateEntity(new InitializeStaticEnemies{ ID = _locationProvider.ID.CalculateHash32() });
            if(_locationProvider.Type == LocationType.Flow) 
                ECSWorld.CreateEntity(new LocationEnemiesFlow{ ID = _locationProvider.ID.CalculateHash32() });
        }

        public void Exit()
        {
            ECSWorld.DestroyScene();
            BattleHUD.Hide();
        }
    }
}    
