using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    [UpdateBefore(typeof(CreateDamageFlyTextEventSystem))]
    public partial struct CreateFlyTextSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);

            foreach (var damageEvent in SystemAPI.Query<RefRO<GetDamageEvent>>())
            {
                var character = damageEvent.ValueRO.Character;
                if (!transforms.HasComponent(character) || !characters.HasComponent(character)) continue;

                var characterConfig = characters[character].GetConfig();
                ecb.CreateFrameEntity(new CreateDamageFlyTextEvent
                {
                    Position = transforms[character].Position + new float3(0, characterConfig.ColliderHeight, 0),
                    Damage = damageEvent.ValueRO.Damage,
                    IsEnemyTarget = SystemAPI.HasComponent<Enemy>(character),
                    IsCrit = damageEvent.ValueRO.IsCrit
                });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
