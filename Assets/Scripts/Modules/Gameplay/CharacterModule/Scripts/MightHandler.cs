using UnityEngine;
using vikwhite.Data;
using vikwhite.ECS;

namespace vikwhite
{
    public static class MightHandler
    {
        public static int Calculate(Character character) => Calculate(character, character.Level.Value, character.Stars.Value);

        public static int Calculate(Character character, int level, int stars)
        {
            float attack = CharacterStatsHandler.Calculate(character, StatType.Attack, level, stars);
            float attackCooldown = character.GetSkill(SkillSlotType.Attack).Config.Cooldown;
            float critChance = CharacterStatsHandler.Calculate(character, StatType.CritChance, level, stars);
            float critValue = CharacterStatsHandler.Calculate(character, StatType.CritValue, level, stars);
            float health = CharacterStatsHandler.Calculate(character, StatType.Health, level, stars);
            float defense = CharacterStatsHandler.Calculate(character, StatType.Defense, level, stars);
            float activeSkillBaseMight = GetSkillMight(character, SkillSlotType.Active);
            float activeSkillLevelMightMultiplier = GetSkillLevelMightMultiplier(character, SkillSlotType.Active);
            float passive1BaseMight = GetSkillMight(character, SkillSlotType.Passive1);
            float passive1LevelMightMultiplier = GetSkillLevelMightMultiplier(character, SkillSlotType.Passive1);
            float passive2BaseMight = GetSkillMight(character, SkillSlotType.Passive2);
            float passive2LevelMightMultiplier = GetSkillLevelMightMultiplier(character, SkillSlotType.Passive2);
            return Calculate(attack, attackCooldown, critChance, critValue, health, defense, activeSkillBaseMight, activeSkillLevelMightMultiplier, passive1BaseMight, passive1LevelMightMultiplier, passive2BaseMight, passive2LevelMightMultiplier);
        }

        public static int Calculate(ICharacterData character, IConfigs configs, int level, int stars, int skillLevel)
        {
            var levelRank = Mathf.Max(0, level - 1);
            var starRank = Mathf.Max(0, stars);
            var levelUpgrade = configs.Upgrades.Get(character.LevelUpgrade);
            var starUpgrade = configs.Upgrades.Get(character.StarUpgrade);

            float attack = GetStat(character, levelUpgrade, starUpgrade, levelRank, starRank, StatType.Attack);
            float critChance = GetStat(character, levelUpgrade, starUpgrade, levelRank, starRank, StatType.CritChance);
            float critValue = GetStat(character, levelUpgrade, starUpgrade, levelRank, starRank, StatType.CritValue);
            float health = GetStat(character, levelUpgrade, starUpgrade, levelRank, starRank, StatType.Health);
            float defense = GetStat(character, levelUpgrade, starUpgrade, levelRank, starRank, StatType.Defense);
            float attackCooldown = GetSkill(configs, character, SkillSlotType.Attack)?.Cooldown ?? 1f;

            GetSkillMight(configs, character, SkillSlotType.Active, skillLevel, out var activeMight, out var activeMultiplier);
            GetSkillMight(configs, character, SkillSlotType.Passive1, skillLevel, out var passive1Might, out var passive1Multiplier);
            GetSkillMight(configs, character, SkillSlotType.Passive2, skillLevel, out var passive2Might, out var passive2Multiplier);

            return Calculate(
                attack,
                Mathf.Max(attackCooldown, 0.01f),
                critChance,
                critValue,
                health,
                defense,
                activeMight,
                activeMultiplier,
                passive1Might,
                passive1Multiplier,
                passive2Might,
                passive2Multiplier);
        }

        private static float GetSkillMight(Character character, SkillSlotType slot)
        {
            var skill = character.GetSkill(slot);
            if (skill != null) return skill.Config.Might;
            return 0;
        }
        
        private static float GetSkillLevelMightMultiplier(Character character, SkillSlotType slot)
        {
            var skill = character.GetSkill(slot);
            if (skill != null) return skill.Config.LevelMightMultiplier * skill.Level.Value + 1;
            return 0;
        }

        private static float GetStat(
            ICharacterData character,
            IUpgradeData levelUpgrade,
            IUpgradeData starUpgrade,
            int levelRank,
            int starRank,
            StatType stat)
        {
            var multiplier = CharacterHandler.GetCompositeMultiplier(
                levelRank,
                starRank,
                levelUpgrade.GetStatMultiplier(stat),
                starUpgrade.GetStatMultiplier(stat));
            return character.GetStat(stat) * multiplier;
        }

        private static ISkillData GetSkill(IConfigs configs, ICharacterData character, SkillSlotType slot)
        {
            var id = character.GetSkill(slot);
            return string.IsNullOrEmpty(id) ? null : configs.Skills.Get(id);
        }

        private static void GetSkillMight(
            IConfigs configs,
            ICharacterData character,
            SkillSlotType slot,
            int skillLevel,
            out float might,
            out float multiplier)
        {
            var skill = GetSkill(configs, character, slot);
            might = skill?.Might ?? 0f;
            multiplier = skill == null ? 0f : skill.LevelMightMultiplier * Mathf.Max(1, skillLevel) + 1f;
        }
        
        private static int Calculate(
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
