using Unity.Entities;

namespace vikwhite.ECS
{
    public static class AbilityHandler
    {
        public static AbilityLevelConfig Get(AbilityID id, int level, DynamicBuffer<AbilityConfig> configs) => configs[(int)id].Levels[level];
    }
}