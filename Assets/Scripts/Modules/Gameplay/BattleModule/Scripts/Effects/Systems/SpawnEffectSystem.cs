using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using vikwhite.Utils;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectSystem))]
    public partial struct SpawnEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (effect, target) in SystemAPI.Query<RefRO<Effect>, RefRO<Target>>().WithAny<SpawnEffect>())
            {
                var character = target.ValueRO.Value;
                var isEnemy = SystemAPI.HasComponent<Enemy>(character);
                var position = SystemAPI.GetComponent<LocalTransform>(character).Position;
                var config = SystemAPI.GetSingletonBuffer<AbilityLevelsConfig>().Get(effect.ValueRO.Ability.ID).Levels.Value.Array[effect.ValueRO.Ability.Level];
                for (int i = 0; i < effect.ValueRO.Value; i++)
                {
                    ecb.CreateFrameEntity(new CreateCharacter
                    {
                        ID = config.GetRandomSpawnCharacter(), 
                        Position = MathHandler.GetRandomPointInRadius(position.xz, config.SpawnRadius).xoy(),
                        IsEnemy = isEnemy
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}