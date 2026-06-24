using UnityEngine;

namespace vikwhite
{
    public static class MightHandler
    {
        public static int Calculate(
            float attack,
            float attackCooldown,
            float critChance,
            float critValue,
            float health,
            float defense,
            float activeSkillBaseMight,
            float activeSkillLevelMightMultiplier,
            float passive1BaseMight,
            float passive1LevelMightMultiplier,
            float passive2BaseMight,
            float passive2LevelMightMultiplier)
        {
            float dps = CalculateDPS(attack, attackCooldown, critChance, critValue);
            float ehp = CalculateEHP(health, defense);
            float skillMultiplier = CalculateSkillMultiplier(activeSkillBaseMight, activeSkillLevelMightMultiplier, passive1BaseMight, passive1LevelMightMultiplier, passive2BaseMight, passive2LevelMightMultiplier);
            return (int)(Mathf.Sqrt(dps * ehp) * skillMultiplier);
        }
        
        private static float CalculateDPS(float attack, float attackCooldown, float critChance, float critValue) 
            => attack / attackCooldown * (1f + critChance * (critValue - 1f));

        private static float CalculateEHP(float health, float defense) 
            => health * (1f + defense / 50f);

        private static float CalculateSkillMultiplier(float activeSkillBaseMight, float activeSkillLevelMightMultiplier, float passive1BaseMight, float passive1LevelMightMultiplier, float passive2BaseMight, float passive2LevelMightMultiplier) 
            => 1f + activeSkillBaseMight * activeSkillLevelMightMultiplier + passive1BaseMight * passive1LevelMightMultiplier + passive2BaseMight * passive2LevelMightMultiplier;
    }
}