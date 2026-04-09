using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct RangeAttackSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (ability, provider, cooldown, entity) in SystemAPI.Query<RefRO<RangeAttack>, RefRO<Provider>, RefRW<Cooldown>>().WithAll<CooldownUp>().WithEntityAccess()) {
                var transform = SystemAPI.GetComponent<LocalToWorld>(provider.ValueRO.Value);
                var forward = math.mul(transform.Rotation, new float3(0, 0, 0.3f));
                ecb.CreateFrameEntity(new CreateBulletProjectile
                {
                    Provider = provider.ValueRO.Value, 
                    Ability = ability.ValueRO.Value, 
                    Position = transform.Position + forward, 
                    Rotation = transform.Rotation,
                });
                ecb.RemoveComponent<CooldownUp>(entity);
                var cooldownMultiply = StatHandler.Get(StatID.CooldownMultiply, SystemAPI.GetSingletonBuffer<StatBase>(), SystemAPI.GetSingletonBuffer<StatMultiply>());
                cooldown.ValueRW.Value = ability.ValueRO.Value.Cooldown * cooldownMultiply;
            }
            ecb.Playback(state.EntityManager);
        }
    }
}