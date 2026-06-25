using Rukhanka.Toolbox;
using vikwhite.Data;

namespace vikwhite
{
    public interface IBattleMightService
    {
        void UpdateEnemyMight();
    }

    public class BattleMightService : IBattleMightService
    {
        private readonly IConfigs _configs;
        private readonly ILocationProvider _location;
        private readonly ISquadService _squad;

        public BattleMightService(IConfigs configs, ILocationProvider location, ISquadService squad)
        {
            _configs = configs;
            _location = location;
            _squad = squad;
        }

        public void UpdateEnemyMight()
        {
            var location = _configs.Map.Get(_location.ID);
            var might = location.Type == LocationType.Static
                ? CalculateStaticLocation()
                : CalculateFlowLocation();
            _squad.SetEnemyMight(might);
        }

        private int CalculateStaticLocation()
        {
            var location = _configs.LocationStatic.Get(_location.ID);
            if (location == null) return 0;

            var might = 0;
            foreach (var enemy in location.Enemies)
                might += CalculateCharacter(enemy.ID, enemy.Level);
            return might;
        }

        private int CalculateFlowLocation()
        {
            var maxMight = 0;
            foreach (var step in _configs.LocationFlow.GetAll())
            {
                if (step.LocationID != _location.ID) continue;

                var strongestEnemy = 0;
                foreach (var enemyID in step.Enemies)
                {
                    var enemy = _configs.Characters.Get(enemyID);
                    if (enemy == null) continue;
                    strongestEnemy = System.Math.Max(
                        strongestEnemy,
                        MightHandler.Calculate(enemy, _configs, 1, 0, 1));
                }

                maxMight = System.Math.Max(maxMight, strongestEnemy * step.Count);
            }
            return maxMight;
        }

        private int CalculateCharacter(uint characterID, int level)
        {
            foreach (var character in _configs.Characters.GetAll())
            {
                if (character.ID.CalculateHash32() != characterID) continue;
                return MightHandler.Calculate(character, _configs, level, 0, 1);
            }
            return 0;
        }
    }
}
