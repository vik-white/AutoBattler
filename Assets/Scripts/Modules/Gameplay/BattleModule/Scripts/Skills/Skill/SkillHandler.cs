using Unity.Entities;

namespace vikwhite.ECS
{
    public static partial class SkillHandler
    {
        public static float GetCooldownRate(uint activeSkillId, uint skillId, DynamicBuffer<StatMultiply> statMultipliers)
        {
            var statType = skillId == activeSkillId ? StatType.SkillActiveCooldown : StatType.SkillAttackCooldown;

            var index = (int)statType;
            if (index < 0 || index >= statMultipliers.Length) return 1f;

            var multiplier = statMultipliers[index].Value;
            return multiplier <= 0f ? 1f : 1f / multiplier;
        }

        public static bool HasActivationAnimation(in SkillConfig skillConfig)
        {
            return skillConfig.Animation is AnimationType.Attack or AnimationType.Ability;
        }
    }
}
