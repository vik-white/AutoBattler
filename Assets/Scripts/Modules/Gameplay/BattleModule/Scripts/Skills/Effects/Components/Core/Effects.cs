using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Effects : IComponentData
    {
        public BlobAssetReference<SkillConfig> Ability;
    }
}
