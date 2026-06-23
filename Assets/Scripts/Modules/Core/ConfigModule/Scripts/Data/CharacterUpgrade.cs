using vikwhite.ECS;

namespace vikwhite.Data
{
    public readonly struct CharacterUpgrade
    {
        public readonly int LevelRank;
        public readonly int StarRank;
        public readonly IUpgradeData LevelUp;
        public readonly IUpgradeData StarUp;

        public CharacterUpgrade(int levelRank, int starRank, IUpgradeData levelUp, IUpgradeData starUp)
        {
            LevelRank = levelRank;
            StarRank = starRank;
            LevelUp = levelUp;
            StarUp = starUp;
        }

        public float GetStatMultiplier(StatType stat) =>
            CharacterHandler.GetCompositeMultiplier(LevelRank, StarRank,
                LevelUp.GetStatMultiplier(stat),
                StarUp.GetStatMultiplier(stat));
    }
}