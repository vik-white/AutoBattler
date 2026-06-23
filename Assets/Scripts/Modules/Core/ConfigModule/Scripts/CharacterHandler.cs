namespace vikwhite
{
    public static class CharacterHandler
    {
        public static float GetUpgradeMultiplier(int rank, float perRank) => rank * perRank + 1;

        public static float GetCompositeMultiplier(int level, int stars, float perLevel, float perStars) =>
            GetUpgradeMultiplier(level, perLevel) * GetUpgradeMultiplier(stars, perStars);
    }
}
