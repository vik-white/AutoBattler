using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Character : IComponentData
    {
        public BlobAssetReference<CharacterConfigData> Config;
        public int Level;
        public int Stars;
        public int SkillLevel;
        public float BaseHealth;
        public float BaseAttack;
        public float BaseDefense;
        public bool UseSummonHealth;
        public bool UseSummonAttack;
        public bool UseSummonDefense;

        public bool TryGetBaseStat(StatType stat, out float value)
        {
            switch (stat)
            {
                case StatType.Health:  value = BaseHealth;  return true;
                case StatType.Attack:  value = BaseAttack;  return true;
                case StatType.Defense: value = BaseDefense; return true;
                default:               value = 0f;          return false;
            }
        }

        public float GetUpgradedBaseStat(in CharacterUpgrade upgrade, StatType stat)
        {
            return TryGetBaseStat(stat, out var value)
                ? value * GetAppliedUpgradeMultiplier(upgrade, stat)
                : 0f;
        }

        public float GetAppliedUpgradeMultiplier(in CharacterUpgrade upgrade, StatType stat)
        {
            var useSummonStat = stat switch
            {
                StatType.Health => UseSummonHealth,
                StatType.Attack => UseSummonAttack,
                StatType.Defense => UseSummonDefense,
                _ => false,
            };
            return useSummonStat ? 1f : upgrade.GetStatMultiplier(stat);
        }
    }

    public static class CharacterExtensions
    {
        public static CharacterConfigData GetConfig(this in Character character)
        {
            return character.Config.Value;
        }
    }
}
