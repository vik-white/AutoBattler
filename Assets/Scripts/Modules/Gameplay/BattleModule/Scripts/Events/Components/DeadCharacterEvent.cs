using Unity.Entities;

namespace vikwhite.ECS
{
    public struct DeadCharacterEvent : IComponentData
    {
        public Entity Character;
    }
}