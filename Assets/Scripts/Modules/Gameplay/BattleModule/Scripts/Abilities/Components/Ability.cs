using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public enum AbilityID
    {
        None = -1,
        RangeAttack = 0,
        MeleeAttack = 1,
        OrbitingFireBoll = 2,
    }
    
    public struct Ability : IBufferElementData
    {
        public AbilityConfig Config;
        public float Cooldown;
        public bool IsReady;
    }
}