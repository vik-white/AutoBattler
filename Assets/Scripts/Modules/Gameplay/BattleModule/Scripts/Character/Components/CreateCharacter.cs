using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateCharacter : IComponentData
    {
        public uint ID;
        public int Level;
        public bool IsEnemy;
        public float3 Position;
    }
}