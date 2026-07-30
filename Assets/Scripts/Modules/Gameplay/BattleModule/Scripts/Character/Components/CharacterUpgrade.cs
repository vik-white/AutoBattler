using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CharacterUpgrade : IComponentData
    {
        public int LevelRank;
        public int StarRank;
        public UpgradeConfig LevelUp;
        public UpgradeConfig StarUp;
        public int BreakthroughLevelPeriod;
        public float BreakthroughMultiply;

        public float GetStatMultiplier(StatType stat) =>
            CharacterHandler.GetCompositeMultiplier(LevelRank, StarRank,
                LevelUp.GetStatMultiplier(stat),
                StarUp.GetStatMultiplier(stat),
                BreakthroughLevelPeriod,
                BreakthroughMultiply);

        public float GetSkillMultiplier(SkillSlotType slot) =>
            CharacterHandler.GetCompositeMultiplier(LevelRank, StarRank,
                LevelUp.GetSkillMultiplier(slot),
                StarUp.GetSkillMultiplier(slot),
                BreakthroughLevelPeriod,
                BreakthroughMultiply);

        public float GetSkillMultiplier(in CharacterConfigData config, uint skillID)
        {
            if (skillID == 0) return 1f;
            if (!config.TryFindSlot(skillID, out var slot) || slot == SkillSlotType.Attack) return 1f;
            return GetSkillMultiplier(slot);
        }
    }
}
