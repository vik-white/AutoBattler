using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public struct CreateAbility : IComponentData
    {
        public Entity Provider;
        public AbilityID ID;
        public int Level;
    }
}