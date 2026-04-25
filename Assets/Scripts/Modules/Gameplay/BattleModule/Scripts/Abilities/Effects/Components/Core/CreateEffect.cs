using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateEffect : IComponentData
    {
        public Entity Provider;
        public Entity Target;
        public EffectData Data;
        public BlobAssetReference<AbilityConfig> Ability;
    }
}
