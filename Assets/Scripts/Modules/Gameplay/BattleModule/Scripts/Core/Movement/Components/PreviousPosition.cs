using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite
{
    public struct PreviousPosition : IComponentData
    {
        public float3 Value;
    }
}