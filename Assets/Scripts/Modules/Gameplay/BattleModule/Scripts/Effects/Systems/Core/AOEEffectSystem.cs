using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(ApplyEffectsOnTargetsSystem))]
    public partial struct AOEEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var abilityLevelConfig = SystemAPI.GetSingletonBuffer<AbilityLevelsConfig>();
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>())
            {
                var target = request.ValueRO.Target;
                var position = SystemAPI.GetComponent<LocalTransform>(target).Position;
                var abilityConfig = abilityLevelConfig.Get(request.ValueRO.Ability.ID).Levels.Value.Array[request.ValueRO.Ability.Level];
                if(abilityConfig.AOE == 0) continue;

                var isEnemy = SystemAPI.HasComponent<Enemy>(target);
                if (isEnemy)
                {
                    foreach (var (transform, enemy) in SystemAPI.Query<RefRO<LocalTransform>>().WithAny<Character, Enemy>().WithEntityAccess())
                    {
                        if(enemy == target) continue;
                        if (math.length(transform.ValueRO.Position - position) <= abilityConfig.AOE)
                        {
                            ecb.CreateFrameEntity(new CreateEffect {
                                Ability = request.ValueRO.Ability,
                                Provider = request.ValueRO.Provider,
                                Target = enemy,
                                Data = request.ValueRO.Data, 
                            });
                        }
                    }
                }
                else
                {
                    foreach (var (transform, ally) in SystemAPI.Query<RefRO<LocalTransform>>().WithAny<Character>().WithNone<Enemy>().WithEntityAccess())
                    {
                        if(ally == target) continue;
                        if (math.length(transform.ValueRO.Position - position) <= abilityConfig.AOE)
                        {
                            ecb.CreateFrameEntity(new CreateEffect {
                                Ability = request.ValueRO.Ability,
                                Provider = request.ValueRO.Provider,
                                Target = ally,
                                Data = request.ValueRO.Data, 
                            });
                        }
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}