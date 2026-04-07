using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateCharacter : IComponentData
    {
        public float3 Position;
    }
}