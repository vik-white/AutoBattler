using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct MeleeAttackSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (abilities, transform, target) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<LocalTransform>, RefRO<Target>>().WithAll<Character>()) {
                foreach (var ability in abilities) {
                    if (ability.Config.ID != AbilityID.MeleeAttack || !ability.IsCooldown) continue;
                    
                    var targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.Value);
                    var direction = targetTransform.Position - transform.ValueRO.Position;
                    var distance = math.length(direction);
                    if (distance > ability.Config.Radius) continue;
                    
                    foreach (var effect in ability.Config.Effects) {
                        ecb.CreateFrameEntity(new CreateEffect {
                            Target = target.ValueRO.Value, 
                            Effect = effect, 
                        });
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}