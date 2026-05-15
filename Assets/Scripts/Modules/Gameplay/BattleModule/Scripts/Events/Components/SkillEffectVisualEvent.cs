using Unity.Entities;

namespace vikwhite.ECS
{
    public struct SkillEffectVisualEvent : IComponentData
    {
        public BlobAssetReference<SkillConfig> Skill;
        public Entity Target;
        public Entity Provider;
    }
}
