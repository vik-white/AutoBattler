using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Statuses : IComponentData
    {
        public AbilityLevelData Ability;
        public FixedList64Bytes<StatusData> Array;
    }
}