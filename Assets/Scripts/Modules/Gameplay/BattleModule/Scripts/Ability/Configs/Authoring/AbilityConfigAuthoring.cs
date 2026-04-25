using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class AbilityConfigAuthoring : MonoBehaviour
    {
        public ConfigsLoader Configs;
    }

    class AbilityConfigAuthoringBaker : Baker<AbilityConfigAuthoring>
    {
        public override void Bake(AbilityConfigAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            var runtimeData = AddBuffer<AbilityRuntimeData>(entity);

            foreach (var abilityData in authoring.Configs.Abilities.GetAll())
            {
                var abilityID = abilityData.AbilityID.CalculateHash32();
                var config = new AbilityConfig
                {
                    ID = abilityID,
                    Level = abilityData.Level,
                    Type = abilityData.Type,
                    Targets = CreateTargets(abilityData.Targets),
                    Cooldown = abilityData.Cooldown,
                    Radius = abilityData.Radius,
                    AOE = abilityData.AOE,
                    Effects = CreateEffects(abilityData.Effects),
                    Statuses = CreateStatuses(abilityData.Statuses),
                    Stats = CreateStats(abilityData.Stats),
                    Projectile = new ProjectileData
                    {
                        Count = abilityData.Count,
                        Speed = abilityData.Speed,
                        Pierce = abilityData.Pierce,
                        Scale = abilityData.Scale,
                        OrbitRadius = abilityData.OrbitRadius,
                        Lifetime = abilityData.Lifetime,
                    },
                    SpawnCharacters = CreateSpawnCharacters(abilityData.SpawnCharacters),
                    SpawnRadius = abilityData.SpawnRadius,
                    AuraLifetime = abilityData.AuraLifetime,
                    AuraRadius = abilityData.AuraRadius,
                    AuraInterval = abilityData.AuraInterval,
                    Abilities = CreateAbilities(abilityData.Abilities),
                    ImpulseUp = abilityData.ImpulseUp,
                    ImpulseProvider = abilityData.ImpulseProvider,
                    CastVFXPrefab =  abilityData.CastVFXPrefab,
                    VFXPrefab =  abilityData.VFXPrefab,
                    VFXSpawn =  abilityData.VFXSpawn,
                    Animation = abilityData.Animation,
                    ProjectilePrefab = abilityData.ProjectilePrefab,
                };

                runtimeData.Add(new AbilityRuntimeData
                {
                    ID = abilityID,
                    Level = abilityData.Level,
                    Config = CreateAbilityConfigBlob(config)
                });
            }
        }

        private static BlobAssetReference<AbilityConfig> CreateAbilityConfigBlob(AbilityConfig config)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<AbilityConfig>();
            root = config;
            return builder.CreateBlobAssetReference<AbilityConfig>(Allocator.Persistent);
        }

        private FixedList64Bytes<EffectData> CreateEffects(List<EffectData> effectConfig) {
            var effects = new FixedList64Bytes<EffectData>();
            foreach (var effect in effectConfig) effects.Add(effect);
            return effects;
        }
        
        private FixedList128Bytes<StatusData> CreateStatuses(List<StatusData> statusConfig) {
            var statuses = new FixedList128Bytes<StatusData>();
            foreach (var status in statusConfig) statuses.Add(status);
            return statuses;
        }
        
        private FixedList128Bytes<StatData> CreateStats(List<StatData> statsConfig) {
            var stats = new FixedList128Bytes<StatData>();
            foreach (var stat in statsConfig) stats.Add(stat);
            return stats;
        }
        
        private FixedList64Bytes<TargetType> CreateTargets(List<TargetType> targetConfig) {
            var targets = new FixedList64Bytes<TargetType>();
            foreach (var target in targetConfig) targets.Add(target);
            return targets;
        }
        
        private FixedList64Bytes<SpawnCharacterData> CreateSpawnCharacters(List<SpawnCharacterData> spawnCharacterConfig) {
            var spawnCharacters = new FixedList64Bytes<SpawnCharacterData>();
            foreach (var spawnCharacter in spawnCharacterConfig) spawnCharacters.Add(spawnCharacter);
            return spawnCharacters;
        }
        
        private FixedList64Bytes<AbilityLevelData> CreateAbilities(List<AbilityLevelData> abilitiesConfig) {
            var abilities = new FixedList64Bytes<AbilityLevelData>();
            foreach (var ability in abilitiesConfig) abilities.Add(ability);
            return abilities;
        }
    }
}
