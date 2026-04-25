using Unity.Burst;
using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    [BurstCompile]
    [UpdateInGroup(typeof(EffectsSystemGroup), OrderFirst = true)]
    public partial struct ApplyEffectsOnTargetsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (collisionTargets, effects, provider) in SystemAPI.Query<DynamicBuffer<CollisionTarget>, RefRO<Effects>, RefRO<Provider>>()) 
            {
                var ability = effects.ValueRO.Ability.Value;
                for (int i = 0; i < collisionTargets.Length; i++) 
                {
                    foreach (var effect in ability.Effects) 
                    {
                        ecb.CreateFrameEntity(new CreateEffect {
                            Ability = effects.ValueRO.Ability,
                            Provider = provider.ValueRO.Value,
                            Target = collisionTargets[i].Value, 
                            Data = effect, 
                        });
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
