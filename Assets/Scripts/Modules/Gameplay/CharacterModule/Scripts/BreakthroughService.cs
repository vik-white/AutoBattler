using vikwhite.Data;

namespace vikwhite
{
    public interface IBreakthroughService
    {
        bool IsRequired(Character character);
        int GetEligibleHeroesCount(Character character);
        bool CanBreakthrough(Character character);
        bool TryBreakthrough(Character character);
    }

    public class BreakthroughService : IBreakthroughService
    {
        private readonly IConfigs _configs;
        private readonly IResourceService _resources;
        private readonly ICharactersService _characters;

        public BreakthroughService(
            IConfigs configs,
            IResourceService resources,
            ICharactersService characters)
        {
            _configs = configs;
            _resources = resources;
            _characters = characters;
        }

        public bool IsRequired(Character character)
        {
            if (character == null) return false;

            var period = _configs.Settings.BreakthroughLevelPeriod;
            var level = character.Level.Value;
            return period > 0
                   && level > 0
                   && level < character.GetMaxLevel()
                   && level % period == 0;
        }

        public int GetEligibleHeroesCount(Character character)
        {
            if (character == null) return 0;

            var threshold = character.Level.Value;
            var count = 0;
            foreach (var hero in _characters.GetCharacters())
            {
                if (hero.Level.Value >= threshold) count++;
            }

            return count;
        }

        public bool CanBreakthrough(Character character)
        {
            if (!IsRequired(character)) return false;

            var settings = _configs.Settings;
            return _resources.GetAmount(ResourceType.Essence).Value >= settings.BreakthroughEssence
                   && _resources.GetAmount(ResourceType.Exp).Value >= settings.BreakthroughExp
                   && GetEligibleHeroesCount(character) >= settings.BreakthroughHeroesCount;
        }

        public bool TryBreakthrough(Character character)
        {
            if (!CanBreakthrough(character)) return false;

            var settings = _configs.Settings;
            _resources.Spend(ResourceType.Essence, settings.BreakthroughEssence);
            _resources.Spend(ResourceType.Exp, settings.BreakthroughExp);
            character.UpgradeLevel();
            return true;
        }
    }
}
