using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public struct Ability : IBufferElementData
    {
        public AbilityLevelConfig Config;
        public float Cooldown;
        public bool IsReady;
    }
}