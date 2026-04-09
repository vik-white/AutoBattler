using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public struct Ability : IBufferElementData
    {
        public AbilityLevelConfig Config;
        public bool IsCooldown;
        public float Cooldown;
    }
}