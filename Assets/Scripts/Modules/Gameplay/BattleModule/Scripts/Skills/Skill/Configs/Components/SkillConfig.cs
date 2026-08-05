using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public struct SkillConfig
    {
        public uint ID;
        public SkillType Type;
        public float Cooldown;
        public int ActivateCount;
        public float Chance;
        public float Radius;
        public float AOE;
        public TriggerType Trigger;
        public TargetType TriggerSource;
        public FixedList64Bytes<TargetType> Targets;
        public int TargetsCount;
        public FixedList64Bytes<TargetConditionType> TargetConditions;
        public FixedList64Bytes<EffectData> Effects;
        public FixedList128Bytes<StatusData> Statuses;
        public FixedList128Bytes<StatData> Stats;
        public ProjectileData Projectile;
        public FixedList64Bytes<SpawnCharacterData> SpawnCharacters;
        public float SpawnRadius;
        public float AuraLifetime;
        public float AuraRadius;
        public float AuraInterval;
        public FixedList64Bytes<uint> Skills;
        public FixedList64Bytes<float> SkillDelays;
        public float ImpulseUp;
        public float ImpulseProvider;
        public uint CastVFXPrefab;
        public uint VFXPrefab;
        public uint ProjectilePrefab;
        public VFXSpawnType VFXSpawn;
        public AnimationType Animation;
        
        public uint GetRandomSpawnCharacter()
        {
            float r = Random.value;
            float cumulative = 0f;
            foreach (var character in SpawnCharacters)
            {
                cumulative += character.Probability;
                if (r <= cumulative) return character.ID;
            }
            return 0;
        }
    }
}
