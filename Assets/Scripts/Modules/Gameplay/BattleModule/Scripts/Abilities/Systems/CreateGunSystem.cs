using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateGunSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateGunAbility>>()) {
                var entity = ecb.CreateEntity();
                var ability = AbilityHandler.Get(AbilityID.Gun, SystemAPI.GetSingletonBuffer<AbilityLevel>(), SystemAPI.GetSingletonBuffer<AbilityConfig>());
                var cooldownMultiply = StatHandler.Get(StatID.CooldownMultiply, SystemAPI.GetSingletonBuffer<StatBase>(), SystemAPI.GetSingletonBuffer<StatMultiply>());
                ecb.AddComponent(entity, new Gun());
                ecb.AddComponent(entity, new Cooldown { Value = ability.Cooldown * cooldownMultiply });
                ecb.AddComponent(entity, new CooldownLeft { Value = 0 });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}