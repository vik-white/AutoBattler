using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateDamageFlyTextEvent : IComponentData
    {
        public float3 Position;
        public float Damage;
    }
}
