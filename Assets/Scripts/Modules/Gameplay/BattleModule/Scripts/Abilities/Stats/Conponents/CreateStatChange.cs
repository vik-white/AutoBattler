using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CreateStatChange : IComponentData
    {
        public Entity Provider;
        public Entity Target;
        public StatData Data;
        public BlobAssetReference<AbilityConfig> Ability;
    }
}
