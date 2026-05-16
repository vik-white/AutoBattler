using Unity.Entities;

namespace vikwhite.ECS
{
    public struct SkillAnimated : IComponentData
    {
        public Entity Trigger;
        public BlobAssetReference<SkillConfig> Skill;
    }
}
