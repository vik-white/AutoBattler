using Unity.Entities;

namespace vikwhite.ECS
{
    public static class AbilityHandler
    {
        public static AbilityConfig Get(AbilityID id, int level, DynamicBuffer<AbilityLevelsConfig> configs) => configs[(int)id].Levels[level];
    }
}