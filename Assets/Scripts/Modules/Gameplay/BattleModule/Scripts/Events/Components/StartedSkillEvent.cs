using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct StartedSkillEvent : IComponentData
    {
        public Entity Character;
        public BlobAssetReference<SkillConfig> Skill;
        public float3 Position;
        public float Speed;
    }
}
