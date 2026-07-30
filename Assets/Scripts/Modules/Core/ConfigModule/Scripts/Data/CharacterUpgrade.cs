using vikwhite.ECS;

namespace vikwhite.Data
{
    public readonly struct CharacterUpgrade
    {
        public readonly int LevelRank;
        public readonly int StarRank;
        public readonly IUpgradeData LevelUp;
        public readonly IUpgradeData StarUp;
        public readonly int BreakthroughLevelPeriod;
        public readonly float BreakthroughMultiply;

        public CharacterUpgrade(
            int levelRank,
            int starRank,
            IUpgradeData levelUp,
            IUpgradeData starUp,
            int breakthroughLevelPeriod,
            float breakthroughMultiply)
        {
            LevelRank = levelRank;
            StarRank = starRank;
            LevelUp = levelUp;
            StarUp = starUp;
            BreakthroughLevelPeriod = breakthroughLevelPeriod;
            BreakthroughMultiply = breakthroughMultiply;
        }

        public float GetStatMultiplier(StatType stat) =>
            CharacterHandler.GetCompositeMultiplier(LevelRank, StarRank,
                LevelUp.GetStatMultiplier(stat),
                StarUp.GetStatMultiplier(stat),
                BreakthroughLevelPeriod,
                BreakthroughMultiply);
    }
}
