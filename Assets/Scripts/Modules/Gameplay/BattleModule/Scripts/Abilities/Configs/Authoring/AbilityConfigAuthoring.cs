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
            var abilityBuffer = AddBuffer<AbilityLevelsConfig>(entity);
            
            
            var abilities = new List<string>();
            foreach (var abilityData in authoring.Configs.Abilities.GetAll())
            {
                if (!abilities.Contains(abilityData.AbilityID)) abilities.Add(abilityData.AbilityID);
            }

            foreach (var abilityID in abilities)
            {
                var steps = new List<AbilityConfig>();
                foreach (var abilityData in authoring.Configs.Abilities.GetAll())
                {
                    if (abilityData.AbilityID != abilityID) continue;
                    var prefab = Resources.Load<GameObject>($"Abilities/Prefabs/{abilityData.Prefab}");
                    steps.Add(new AbilityConfig
                    {
                        ID = abilityID.CalculateHash32(),
                        Type = abilityData.Type,
                        Targets = CreateTargets(abilityData.Targets),
                        Prefab = this.RegisterPrefab(prefab),
                        Cooldown = abilityData.Cooldown,
                        Radius = abilityData.Radius,
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
                    });

                }
                
                var abilityLevelsConfig = new AbilityLevelsConfig
                {
                    ID = abilityID.CalculateHash32(),
                    Type = steps[0].Type,
                    Levels = ArrayHandler.CreateBlobArray(steps.Count, e => steps[e]),
                };
                abilityBuffer.Add(abilityLevelsConfig);
            }
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
    }
}