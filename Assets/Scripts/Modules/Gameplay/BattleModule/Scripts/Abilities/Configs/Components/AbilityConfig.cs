using Unity.Collections;
using Unity.Entities;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public struct AbilityConfig
    {
        public uint ID;
        public AbilityType Type;
        public int Prefab;
        public float Cooldown;
        public float Radius;
        public FixedList64Bytes<TargetType> Targets;
        public FixedList64Bytes<EffectData> Effects;
        public FixedList128Bytes<StatusData> Statuses;
        public FixedList128Bytes<StatData> Stats;
        public ProjectileData Projectile;
    }
}