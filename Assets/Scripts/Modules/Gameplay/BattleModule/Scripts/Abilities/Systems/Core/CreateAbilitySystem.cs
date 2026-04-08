using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateAbility>>()) {
                var entity = ecb.CreateEntity();
                if (request.ValueRO.Value == AbilityID.Gun) {
                    var ability = AbilityHandler.Get(request.ValueRO.Value, SystemAPI.GetSingletonBuffer<AbilityLevel>(), SystemAPI.GetSingletonBuffer<AbilityConfig>());
                    var cooldownMultiply = StatHandler.Get(StatID.CooldownMultiply, SystemAPI.GetSingletonBuffer<StatBase>(), SystemAPI.GetSingletonBuffer<StatMultiply>());
                    ecb.AddComponent(entity, new Gun());
                    ecb.AddComponent(entity, new Cooldown { Value = ability.Cooldown * cooldownMultiply });
                    ecb.AddComponent(entity, new CooldownLeft { Value = 0 });
                }
                if(request.ValueRO.Value == AbilityID.DamageMultiply) ecb.AddComponent(entity, new StatAbility { ID = AbilityID.DamageMultiply });
                if(request.ValueRO.Value == AbilityID.CooldownMultiply) ecb.AddComponent(entity, new StatAbility { ID = AbilityID.CooldownMultiply });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}