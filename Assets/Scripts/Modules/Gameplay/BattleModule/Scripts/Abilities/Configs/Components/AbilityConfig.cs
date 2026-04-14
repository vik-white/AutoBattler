using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public struct AbilityConfig
    {
        public uint ID;
        public int Level;
        public AbilityType Type;
        public int Prefab;
        public float Cooldown;
        public float Radius;
        public FixedList64Bytes<TargetType> Targets;
        public FixedList64Bytes<EffectData> Effects;
        public FixedList128Bytes<StatusData> Statuses;
        public FixedList128Bytes<StatData> Stats;
        public ProjectileData Projectile;
        public FixedList64Bytes<SpawnCharacterData> SpawnCharacters;
        public float SpawnRadius;
        public float AuraLifetime;
        public float AuraRadius;
        public float AuraInterval;
        
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