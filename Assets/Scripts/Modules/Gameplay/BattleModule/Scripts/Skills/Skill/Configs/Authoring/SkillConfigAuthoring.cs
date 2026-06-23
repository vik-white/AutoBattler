using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class SkillConfigAuthoring : MonoBehaviour
    {
        public ConfigsLoader Configs;
    }

    class SkillConfigAuthoringBaker : Baker<SkillConfigAuthoring>
    {
        public override void Bake(SkillConfigAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            var runtimeData = AddBuffer<SkillRuntimeData>(entity);

            foreach (var data in authoring.Configs.Skills.GetAll())
            {
                var config = new SkillConfig
                {
                    ID = data.ID.CalculateHash32(),
                    Type = data.Type,
                    Targets = CreateTargets(data.Targets),
                    Cooldown = data.Cooldown,
                    Chance = data.Chance,
                    Radius = data.Radius,
                    AOE = data.AOE,
                    Trigger = data.Trigger,
                    TriggerSource = data.TriggerSource,
                    Effects = CreateEffects(data.Effects),
                    Statuses = CreateStatuses(data.Statuses),
                    Stats = CreateStats(data.Stats),
                    Projectile = new ProjectileData
                    {
                        Count = data.Count,
                        Speed = data.Speed,
                        Pierce = data.Pierce,
                        Scale = data.Scale,
                        OrbitRadius = data.OrbitRadius,
                        Lifetime = data.Lifetime,
                    },
                    SpawnCharacters = CreateSpawnCharacters(data.SpawnCharacters),
                    SpawnRadius = data.SpawnRadius,
                    AuraLifetime = data.AuraLifetime,
                    AuraRadius = data.AuraRadius,
                    AuraInterval = data.AuraInterval,
                    Skills = CreateSkills(data.Skills),
                    ImpulseUp = data.ImpulseUp,
                    ImpulseProvider = data.ImpulseProvider,
                    CastVFXPrefab =  data.CastVFXPrefab,
                    VFXPrefab =  data.VFXPrefab,
                    VFXSpawn =  data.VFXSpawn,
                    Animation = data.Animation,
                    ProjectilePrefab = data.ProjectilePrefab,
                };

                runtimeData.Add(new SkillRuntimeData
                {
                    Config = CreateSkillConfigBlob(config)
                });
            }
        }

        private BlobAssetReference<SkillConfig> CreateSkillConfigBlob(SkillConfig config)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<SkillConfig>();
            root = config;
            var blob = builder.CreateBlobAssetReference<SkillConfig>(Allocator.Persistent);
            AddBlobAsset(ref blob, out _);
            return blob;
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
        
        private FixedList64Bytes<uint> CreateSkills(List<uint> skillsConfig) {
            var skills = new FixedList64Bytes<uint>();
            foreach (var skill in skillsConfig) skills.Add(skill);
            return skills;
        }
    }
}
