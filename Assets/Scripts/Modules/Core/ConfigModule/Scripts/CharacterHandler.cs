namespace vikwhite
{
    public static class CharacterHandler
    {
        public static float GetUpgradeMultiplier(float rank, float perRank) => rank * perRank + 1;

        public static float GetEffectiveLevelRank(
            int levelRank,
            int breakthroughLevelPeriod,
            float breakthroughMultiply)
        {
            if (levelRank <= 0) return 0f;
            if (breakthroughLevelPeriod <= 0) return levelRank;
            var breakthroughCount = levelRank / breakthroughLevelPeriod;
            return levelRank + breakthroughCount * (breakthroughMultiply - 1f);
        }

        public static float GetCompositeMultiplier(
            int levelRank,
            int stars,
            float perLevel,
            float perStars,
            int breakthroughLevelPeriod,
            float breakthroughMultiply)
        {
            var effectiveLevelRank = GetEffectiveLevelRank(
                levelRank,
                breakthroughLevelPeriod,
                breakthroughMultiply);
            return GetUpgradeMultiplier(effectiveLevelRank, perLevel)
                   * GetUpgradeMultiplier(stars, perStars);
        }
    }
}
