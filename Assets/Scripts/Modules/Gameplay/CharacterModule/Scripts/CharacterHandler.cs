namespace vikwhite
{
    public static class CharacterHandler
    {
        public static float GetLevelMultiplier(int rank, float perRank) => rank * perRank + 1;

        public static float GetCompositeMultiplier(int level, int stars, int skillLevel, float perLevel, float perStars, float perSkill) =>
            GetLevelMultiplier(level, perLevel) *
            GetLevelMultiplier(stars, perStars) *
            GetLevelMultiplier(skillLevel, perSkill);
    }
}
