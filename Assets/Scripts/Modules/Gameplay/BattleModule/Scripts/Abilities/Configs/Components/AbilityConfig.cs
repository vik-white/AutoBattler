using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct AbilityConfig : IBufferElementData
    {
        public AbilityID ID;
        public FixedList4096Bytes<AbilityLevelConfig> Levels;
    }
}