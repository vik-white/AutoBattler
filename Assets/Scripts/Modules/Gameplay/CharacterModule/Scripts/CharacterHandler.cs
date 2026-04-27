namespace vikwhite
{
    public static class CharacterHandler
    {
        public static float GetLevelMultiplier(int level, float multiply) => level * multiply + 1;
    }
}