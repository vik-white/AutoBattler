using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite
{
    public struct ExternalVelocity : IComponentData
    {
        public float3 Value;
    }
}