using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct RangeAttackAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (abilities, transform, entity) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<LocalTransform>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var ability in abilities) {
                    if (ability.Config.Type != AbilityType.RangeAttack || !ability.IsActivate) continue;
                    
                    var forward = math.mul(transform.ValueRO.Rotation, new float3(0, 0, 0.3f));
                    ecb.CreateFrameEntity(new CreateBulletProjectile
                    {
                        Provider = entity, 
                        Ability = ability.Config, 
                        Position = transform.ValueRO.Position + forward,
                        Rotation = transform.ValueRO.Rotation,
                    });
                    ecb.CreateFrameEntity(new Animation { Character = entity, ID = AnimationID.Attack });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}