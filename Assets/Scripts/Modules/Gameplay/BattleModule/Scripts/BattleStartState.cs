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
            ECSWorld.CreateEntity(new StartStaticLocation{ ID = _locationProvider.Location.CalculateHash32() });
        }

        public void Exit()
        {
            ECSWorld.DestroyScene();
            BattleHUD.Hide();
        }
    }
}    
