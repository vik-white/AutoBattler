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
            foreach (var (abilities, localToWorld, target, entity) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<LocalToWorld>, RefRO<Target>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var ability in abilities) {
                    if (ability.Config.ID != AbilityID.RangeAttack || !ability.IsCooldown) continue;
                    
                    var targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.Value);
                    var direction = targetTransform.Position - localToWorld.ValueRO.Position;
                    var distance = math.length(direction);
                    if (distance > ability.Config.Radius) continue;
                    
                    var forward = math.mul(localToWorld.ValueRO.Rotation, new float3(0, 0, 0.3f));
                    ecb.CreateFrameEntity(new CreateBulletProjectile
                    {
                        Provider = entity, 
                        Ability = ability.Config, 
                        Position = localToWorld.ValueRO.Position + forward,
                        Rotation = localToWorld.ValueRO.Rotation,
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}