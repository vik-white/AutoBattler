using Unity.Entities;

namespace vikwhite.ECS
{
    public static class AbilityHandler
    {
        public static AbilityLevelConfig Get(AbilityID id, DynamicBuffer<AbilityLevel> levels, DynamicBuffer<AbilityConfig> configs) => configs[(int)id].Levels[levels[(int)id].Value];
    }
}