using Unity.Entities;

namespace vikwhite.ECS
{
    public struct StarterSkill : IBufferElementData
    {
        public Entity Trigger;
        public BlobAssetReference<SkillConfig> Skill;
        public bool WaitForAnimation;
    }
}
