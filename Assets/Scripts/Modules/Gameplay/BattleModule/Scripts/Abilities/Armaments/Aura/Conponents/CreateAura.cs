using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CreateAura : IComponentData
    {
        public Entity Provider;
        public BlobAssetReference<AbilityConfig> Ability;
    }
}
