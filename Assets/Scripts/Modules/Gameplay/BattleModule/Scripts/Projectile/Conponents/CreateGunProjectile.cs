using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateGunProjectile : IComponentData
    {
        public float3 Position;
        public quaternion Rotation;
    }
}