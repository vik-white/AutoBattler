using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Effect : IComponentData
    {
        public BlobAssetReference<SkillConfig> Ability;
        public float Value;
        public bool IsCrit;
    }
}
