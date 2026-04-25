using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public struct Ability : IBufferElementData
    {
        public BlobAssetReference<AbilityConfig> Config;
        public float Cooldown;
        public bool IsActivate;
        public bool IsAnimation;
        public bool IsChild;
    }

    public static class AbilityExtensions
    {
        public static AbilityConfig GetConfig(this in Ability ability)
        {
            return ability.Config.Value;
        }
    }
}
