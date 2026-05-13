using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateBulletProjectile : IComponentData
    {
        public Entity Provider;
        public BlobAssetReference<SkillConfig> Skill;
        public float3 Position;
        public quaternion Rotation;
    }
}
