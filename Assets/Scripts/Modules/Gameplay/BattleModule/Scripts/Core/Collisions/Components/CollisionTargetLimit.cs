using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CollisionTargetLimit : IComponentData
    {
        public int Value;
    }
}