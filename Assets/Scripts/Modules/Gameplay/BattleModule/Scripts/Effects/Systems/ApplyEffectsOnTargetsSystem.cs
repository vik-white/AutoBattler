using Unity.Burst;
using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    [BurstCompile]
    [UpdateInGroup(typeof(EffectsSystemGroup), OrderFirst = true)]
    public partial struct ApplyEffectsOnTargetsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (collisionTargets, effects) in SystemAPI.Query<
                         DynamicBuffer<CollisionTarget>, 
                         DynamicBuffer<EffectData>>()) {
                for (int i = 0; i < collisionTargets.Length; i++) {
                    foreach (var effect in effects) {
                        ecb.CreateFrameEntity(new CreateEffect {
                            Target = collisionTargets[i].Value, 
                            Effect = effect, 
                        });
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}