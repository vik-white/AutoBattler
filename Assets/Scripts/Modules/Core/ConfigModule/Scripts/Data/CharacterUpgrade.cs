using vikwhite.ECS;

namespace vikwhite.Data
{
    public readonly struct CharacterUpgrade
    {
        public readonly int LevelRank;
        public readonly int StarRank;
        public readonly int SkillRank;
        public readonly IUpgradeData LevelUp;
        public readonly IUpgradeData StarUp;
        public readonly IUpgradeData SkillUp;

        public CharacterUpgrade(int levelRank, int starRank, int skillRank, IUpgradeData levelUp, IUpgradeData starUp, IUpgradeData skillUp)
        {
            LevelRank = levelRank;
            StarRank = starRank;
            SkillRank = skillRank;
            LevelUp = levelUp;
            StarUp = starUp;
            SkillUp = skillUp;
        }

        public float GetStatMultiplier(StatType stat) =>
            CharacterHandler.GetCompositeMultiplier(LevelRank, StarRank, SkillRank,
                LevelUp.GetStatMultiplier(stat),
                StarUp.GetStatMultiplier(stat),
                SkillUp.GetStatMultiplier(stat));

        public float GetSkillMultiplier(SkillSlotType slot) =>
            CharacterHandler.GetCompositeMultiplier(LevelRank, StarRank, SkillRank,
                LevelUp.GetSkillMultiplier(slot),
                StarUp.GetSkillMultiplier(slot),
                SkillUp.GetSkillMultiplier(slot));
    }
}