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
            foreach (var (abilities, target) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<Target>>().WithAll<Character>()) {
                for (int i = 0; i < abilities.Length; i++){
                    ref var ability = ref abilities.ElementAt(i);
                    if (ability.Config.ID != AbilityID.MeleeAttack || !ability.IsReady) continue;
                    ability.Cooldown = 0;
                    
                    foreach (var effect in ability.Config.Effects) {
                        ecb.CreateFrameEntity(new CreateEffect 
                        {
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