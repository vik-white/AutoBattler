using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct AbilityLevelsConfig : IBufferElementData
    {
        public AbilityID ID;
        public FixedList4096Bytes<AbilityConfig> Levels;
    }
}