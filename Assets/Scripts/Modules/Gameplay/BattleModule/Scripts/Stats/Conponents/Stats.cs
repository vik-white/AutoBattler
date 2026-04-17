using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Stats : IComponentData
    {
        public AbilityLevelData Ability;
        public FixedList64Bytes<StatData> Array;
    }
}