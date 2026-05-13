using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Effect : IComponentData
    {
        public BlobAssetReference<SkillConfig> Skill;
        public float Value;
        public bool IsCrit;
    }
}
