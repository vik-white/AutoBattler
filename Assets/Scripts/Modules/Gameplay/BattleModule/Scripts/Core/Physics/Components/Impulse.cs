using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite
{
    public struct Impulse : IComponentData
    {
        public float3 Value;
    }
}