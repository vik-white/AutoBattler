using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Effect : IComponentData
    {
        public BlobAssetReference<AbilityConfig> Ability;
        public float Value;
    }
}
