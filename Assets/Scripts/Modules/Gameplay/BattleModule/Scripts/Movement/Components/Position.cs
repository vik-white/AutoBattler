using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct Position : IComponentData
    {
        public float3 Value;
    }
}