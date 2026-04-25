using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CollisionTarget : IBufferElementData
    {
        public Entity Value;
    }
}