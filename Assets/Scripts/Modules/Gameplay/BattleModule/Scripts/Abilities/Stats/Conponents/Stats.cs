using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Stats : IComponentData
    {
        public BlobAssetReference<AbilityConfig> Ability;
    }
}
