using Unity.Entities;

namespace vikwhite.ECS
{
    public struct ActiveAbility : IComponentData
    {
        public AbilityID Value;
    }
}