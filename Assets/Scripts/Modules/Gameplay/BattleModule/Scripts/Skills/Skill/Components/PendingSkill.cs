using Unity.Entities;

namespace vikwhite.ECS
{
    public struct PendingSkill : IBufferElementData
    {
        public Entity Trigger;
        public BlobAssetReference<SkillConfig> Skill;
        public bool WaitForAnimation;
    }
}
