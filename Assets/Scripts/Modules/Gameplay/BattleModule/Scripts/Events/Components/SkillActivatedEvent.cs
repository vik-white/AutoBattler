using Unity.Entities;

namespace vikwhite.ECS
{
    public struct SkillActivatedEvent : IComponentData
    {
        public Entity Character;
        public BlobAssetReference<SkillConfig> Skill;
    }
}
