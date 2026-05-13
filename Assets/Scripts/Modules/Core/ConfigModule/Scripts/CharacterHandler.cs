namespace vikwhite
{
    public static class CharacterHandler
    {
        public static float GetUpgradeMultiplier(int rank, float perRank) => rank * perRank + 1;

        public static float GetCompositeMultiplier(int level, int stars, int skillLevel, float perLevel, float perStars, float perSkill) =>
            GetUpgradeMultiplier(level, perLevel) * GetUpgradeMultiplier(stars, perStars) * GetUpgradeMultiplier(skillLevel, perSkill);
    }
}
