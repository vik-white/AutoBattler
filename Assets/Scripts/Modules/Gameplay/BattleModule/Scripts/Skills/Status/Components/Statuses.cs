using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Statuses : IComponentData
    {
        public BlobAssetReference<SkillConfig> Ability;
    }
}
