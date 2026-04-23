using Rukhanka.Toolbox;
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

        public BattleStartState(ILocationProvider locationProvider, ISquadService squad)
        {
            _locationProvider = locationProvider;
            _squad = squad;
        }

        public void Enter()
        {
            BattleHUD.Show();
            ECSWorld.Enable<InitializeTimeSystem>();
            ECSWorld.Enable<VFXConfigInitializeSystem>();
            ECSWorld.Enable<CharacterConfigInitializeSystem>();
            ECSWorld.CreateEntity(new InitializeSquad{ Value = _squad.GetCharactersHash() });
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
