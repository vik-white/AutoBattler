using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateCharacter : IComponentData
    {
        public uint ID;
        public float3 Position;
        public bool IsEnemy;
    }
}