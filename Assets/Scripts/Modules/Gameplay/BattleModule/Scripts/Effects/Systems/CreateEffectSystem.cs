using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(ApplyEffectsOnTargetsSystem))]
    public partial struct CreateEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>()) {
                var id = request.ValueRO.Effect.Type;
                var effect = ecb.CreateEntity();
                ecb.AddComponent<Effect>(effect);
                ecb.AddComponent(effect, new EffectValue{ Value = GetEffectValue(request.ValueRO.Effect) });
                ecb.AddComponent(effect, new Target{ Value = request.ValueRO.Target });
                ecb.AddComponent(effect, new Position{ Value = request.ValueRO.Position });
                if (id == EffectType.Damage) ecb.AddComponent<EffectDamage>(effect);
            }
            ecb.Playback(state.EntityManager);
        }

        public float GetEffectValue(EffectData effect) {
            if (effect.Type == EffectType.Damage) return effect.Value * StatHandler.Get(StatType.DamageMultiply, SystemAPI.GetSingletonBuffer<StatBase>(), SystemAPI.GetSingletonBuffer<StatMultiply>());
            return effect.Value;
        }
    }
}