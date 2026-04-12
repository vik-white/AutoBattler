using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public struct Ability : IBufferElementData
    {
        public AbilityConfig Config;
        public float Cooldown;
        public bool IsActivate;
    }
}