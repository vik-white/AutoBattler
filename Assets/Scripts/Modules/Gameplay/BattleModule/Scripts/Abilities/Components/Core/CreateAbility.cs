using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public struct CreateAbility : IComponentData
    {
        public AbilityID Value;
    }
}