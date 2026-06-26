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
            var might = location.Type == LocationType.Static ? CalculateStaticLocation() : 0;
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
