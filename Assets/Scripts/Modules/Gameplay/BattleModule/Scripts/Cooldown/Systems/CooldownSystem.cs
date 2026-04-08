using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct CooldownSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (cooldownLeft, cooldown, entity) in SystemAPI.Query<RefRW<CooldownLeft>, RefRO<Cooldown>>().WithNone<CooldownUp>().WithEntityAccess()) {
                cooldownLeft.ValueRW.Value -= SystemAPI.GetSingleton<Time>().DeltaTime;
                if (cooldownLeft.ValueRO.Value <= 0) {
                    ecb.AddComponent<CooldownUp>(entity);
                    cooldownLeft.ValueRW.Value = cooldown.ValueRO.Value;
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}