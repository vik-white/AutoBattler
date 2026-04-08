using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct GunSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (cooldown, entity) in SystemAPI.Query<RefRW<Cooldown>>().WithAll<CooldownUp, Gun>().WithEntityAccess()) {
                /*var cameraTransform = SystemAPI.GetComponent<LocalToWorld>(SystemAPI.GetSingletonEntity<MainEntityCamera>());
                var forward = math.mul(cameraTransform.Rotation, new float3(0, 0, 0.3f));
                ecb.CreateFrameEntity(new CreateGunProjectile { Position = cameraTransform.Position + forward, Rotation = cameraTransform.Rotation, });
                ecb.RemoveComponent<CooldownUp>(entity);
                var ability = AbilityHandler.Get(AbilityID.Gun, SystemAPI.GetSingletonBuffer<AbilityLevel>(), SystemAPI.GetSingletonBuffer<AbilityConfig>());
                var cooldownMultiply = StatHandler.Get(StatID.CooldownMultiply, SystemAPI.GetSingletonBuffer<StatBase>(), SystemAPI.GetSingletonBuffer<StatMultiply>());
                cooldown.ValueRW.Value = ability.Cooldown * cooldownMultiply;*/
            }
            ecb.Playback(state.EntityManager);
        }
    }
}