using Unity.Entities;

namespace vikwhite.ECS
{
    public struct SkillActivatedEvent : IComponentData
    {
        public Entity Character;
        public Entity TriggerSource;
        public Entity Trigger;
        public BlobAssetReference<SkillConfig> Skill;
    }
}
