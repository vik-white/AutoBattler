using UnityEngine;
using vikwhite.Data;
using vikwhite.ECS;

namespace vikwhite
{
    public static class CharacterStatsHandler
    {
        public static float Calculate(Character character, StatType stat) => Calculate(character, stat, character.Level.Value, character.Stars.Value);

        public static float Calculate(Character character, StatType stat, int level, int stars)
        {
            var upgrade = new CharacterUpgrade(Mathf.Max(0, level - 1), Mathf.Max(0, stars), character.LevelUpgrade, character.StarUpgrade);
            return character.Config.GetStat(stat) * upgrade.GetStatMultiplier(stat);
        }
    }
}