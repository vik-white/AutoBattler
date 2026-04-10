using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateBulletProjectile : IComponentData
    {
        public Entity Provider;
        public AbilityConfig Ability;
        public float3 Position;
        public quaternion Rotation;
    }
}