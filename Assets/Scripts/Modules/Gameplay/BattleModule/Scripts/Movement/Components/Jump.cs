using Unity.Mathematics;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Jump : IComponentData
    {
        public Entity Value;
        public float3 StartPosition;
        public float Progress;
        public float Duration;
        public float Height;
    }
}
