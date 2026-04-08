using Unity.Collections;
using Unity.Entities;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public struct AbilityLevelConfig
    {
        public int Prefab;
        public float Cooldown;
        public FixedList64Bytes<StatData> Stats;
        public FixedList64Bytes<EffectData> Effects;
        public ProjectileData Projectile;
    }
}