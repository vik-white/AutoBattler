using Unity.Entities;

namespace vikwhite.ECS
{
    public struct RangeAttack : IComponentData
    {
        public AbilityLevelConfig Value;
    }
}