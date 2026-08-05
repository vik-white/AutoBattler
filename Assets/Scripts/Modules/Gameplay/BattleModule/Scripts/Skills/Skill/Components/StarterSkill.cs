using Unity.Entities;

namespace vikwhite.ECS
{
    public struct StarterSkill : IBufferElementData
    {
        public Entity TriggerSource;
        public Entity Trigger;
        public BlobAssetReference<SkillConfig> Skill;
        public bool WaitForAnimation;
    }
}
