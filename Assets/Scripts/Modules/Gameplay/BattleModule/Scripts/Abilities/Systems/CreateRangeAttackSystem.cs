using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateRangeAttackSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateAbility>>()) {
                if(request.ValueRO.ID != AbilityID.RangeAttack) continue;
                var entity = ecb.CreateSceneEntity();
                var ability = AbilityHandler.Get(AbilityID.RangeAttack, request.ValueRO.Level, SystemAPI.GetSingletonBuffer<AbilityConfig>());
                var cooldownMultiply = StatHandler.Get(StatID.CooldownMultiply, SystemAPI.GetSingletonBuffer<StatBase>(), SystemAPI.GetSingletonBuffer<StatMultiply>());
                ecb.AddComponent<Ability>(entity);
                ecb.AddComponent(entity, new Provider{ Value = request.ValueRO.Provider });
                ecb.AddComponent(entity, new RangeAttack{ Value = ability });
                ecb.AddComponent(entity, new Cooldown { Value = ability.Cooldown * cooldownMultiply });
                ecb.AddComponent(entity, new CooldownLeft { Value = 0 });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}