using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateOrbitProjectile : IComponentData
    {
        public Entity Provider;
        public BlobAssetReference<AbilityConfig> Ability;
        public float3 Position;
        public float Phase;
    }
}
