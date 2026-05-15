using Unity.Entities;

namespace vikwhite.ECS
{
    public struct PendingSkillActivation : IComponentData
    {
        public Entity Character;
        public BlobAssetReference<SkillConfig> Skill;
    }
}
