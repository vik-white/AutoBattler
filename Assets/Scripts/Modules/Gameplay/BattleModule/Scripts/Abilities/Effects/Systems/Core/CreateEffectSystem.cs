using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectImpulseSystem))]
    public partial struct CreateEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var levelUpConfigs = SystemAPI.GetSingleton<LevelUpConfigsBlob>().Value;
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>()) {
                var type = request.ValueRO.Data.Type;
                var effect = ecb.CreateEntity();
                ecb.AddComponent(effect, new Effect
                {
                    Ability = request.ValueRO.Ability,
                    Value = GetEffectValue(ref state, levelUpConfigs, request.ValueRO.Data, request.ValueRO.Provider)
                });
                ecb.AddComponent(effect, new Target{ Value = request.ValueRO.Target });
                ecb.AddComponent(effect, new Provider{ Value = request.ValueRO.Provider });
                if (type == EffectType.Damage) ecb.AddComponent<DamageEffect>(effect);
                if (type == EffectType.Heal) ecb.AddComponent<HealEffect>(effect);
                if (type == EffectType.Shield) ecb.AddComponent<ShieldEffect>(effect);
                if (type == EffectType.Spawn) ecb.AddComponent<SpawnEffect>(effect);
                if (type == EffectType.Aggro) ecb.AddComponent<AggroEffect>(effect);
            }
            ecb.Playback(state.EntityManager);
        }

        public float GetEffectValue(ref SystemState state, BlobAssetReference<BlobArrayContainer<LevelUpConfig>> levelUpConfigs, EffectData effect, Entity entity) {
            var value = effect.Value;
            var character = SystemAPI.GetComponent<Character>(entity);
            var level = character.Level;
            var levelUpConfig = levelUpConfigs.Get(character.GetConfig().LevelUp);

            if (effect.Type == EffectType.Damage)
            {
                value *= CharacterHandler.GetLevelMultiplier(level, levelUpConfig.Damage);
                value *= SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.DamageMultiply].Value;
                return value;
            }

            if (effect.Type == EffectType.Heal)
                return value * CharacterHandler.GetLevelMultiplier(level, levelUpConfig.Heal);

            if (effect.Type == EffectType.Shield)
                return value * CharacterHandler.GetLevelMultiplier(level, levelUpConfig.Shield);

            return value;
        }
    }
}
