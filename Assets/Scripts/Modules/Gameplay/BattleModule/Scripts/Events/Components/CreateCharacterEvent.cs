using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CreateCharacterEvent : IComponentData
    {
        public Entity Character;
    }
}