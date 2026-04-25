using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CreateStatus : IComponentData
    {
        public Entity Provider;
        public Entity Target;
        public StatusData Data;
        public BlobAssetReference<AbilityConfig> Ability;
    }
}
