using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class AbilityConfigAuthoring : MonoBehaviour { }

    class AbilityConfigAuthoringBaker : Baker<AbilityConfigAuthoring>
    {
        public override void Bake(AbilityConfigAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            var abilities = AddBuffer<AbilityConfig>(entity);
            foreach (var ability in AbilitiesSO.Instance.Array) {
                abilities.Add(new AbilityConfig {
                    ID = ability.ID,
                    Levels = CreateAbilityLevels(ability.Levels),
                });
            }
        }

        private FixedList4096Bytes<AbilityLevelConfig> CreateAbilityLevels(List<AbilityLevelData> levelConfig) {
            var levels = new FixedList4096Bytes<AbilityLevelConfig>();
            foreach (var level in levelConfig) {
                levels.Add(new AbilityLevelConfig {
                    Prefab = this.RegisterPrefab(level.Prefab),
                    Cooldown = level.Cooldown,
                    Projectile = level.Projectile,
                    Effects = CreateEffects(level.Effects),
                    Stats = CreateStats(level.Stats)
                });
            }
            return levels;
        }
        
        private FixedList64Bytes<EffectData> CreateEffects(List<EffectData> effectConfig) {
            var effects = new FixedList64Bytes<EffectData>();
            foreach (var effect in effectConfig) effects.Add(effect);
            return effects;
        }
        
        private FixedList64Bytes<StatData> CreateStats(List<StatData> statsConfig) {
            var stats = new FixedList64Bytes<StatData>();
            foreach (var stat in statsConfig) stats.Add(stat);
            return stats;
        }
    }
}