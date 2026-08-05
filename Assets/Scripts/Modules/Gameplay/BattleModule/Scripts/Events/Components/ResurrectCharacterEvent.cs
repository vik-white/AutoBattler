using Unity.Entities;

namespace vikwhite.ECS
{
    public struct ResurrectCharacterEvent : IComponentData
    {
        public Entity Character;
    }
}
