using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace vikwhite.ECS
{
    [BurstCompile]
    [UpdateInGroup(typeof(CollisionSystemGroup))]
    public partial struct CharacterCollisionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var job = new CharacterCollisionJob
            {
                Characters = SystemAPI.GetComponentLookup<Character>(true),
                CollisionTargetLimits = SystemAPI.GetComponentLookup<CollisionTargetLimit>(),
                CollisionTargets = SystemAPI.GetBufferLookup<CollisionTarget>(),
                CollisionBuffers = SystemAPI.GetBufferLookup<CollisionBuffer>(),
                Providers = SystemAPI.GetComponentLookup<Provider>(true),
            };
            state.Dependency = job.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }
    }

    [BurstCompile]
    struct CharacterCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<Character> Characters;
        public ComponentLookup<CollisionTargetLimit> CollisionTargetLimits;
        public BufferLookup<CollisionTarget> CollisionTargets;
        public BufferLookup<CollisionBuffer> CollisionBuffers;
        [ReadOnly] public ComponentLookup<Provider> Providers;

        public void Execute(TriggerEvent triggerEvent)
        {
            var a = triggerEvent.EntityA;
            var b = triggerEvent.EntityB;
            
            bool aIsTarget = CollisionTargetLimits.HasComponent(a);
            bool bIsTarget = CollisionTargetLimits.HasComponent(b);

            bool aIsCharacter = Characters.HasComponent(a);
            bool bIsCharacter = Characters.HasComponent(b);

            if ((aIsTarget && bIsCharacter) || (bIsTarget && aIsCharacter))
            {
                var target = aIsTarget ? a : b;
                var character = aIsCharacter ? a : b;

                if (CollisionTargetLimits[target].Value > 0)
                {
                    if (Providers.TryGetComponent(target, out var provider) && provider.Value != character)
                    {
                        var collisionBuffer = CollisionBuffers[target];
                        var isContains = false;
                        foreach (var collision in collisionBuffer) {
                            if (collision.Value == character) {
                                isContains =  true;
                                break;
                            }
                        }
                        if (!isContains) {
                            collisionBuffer.Add(new CollisionBuffer { Value = character });
                            CollisionTargets[target].Add(new CollisionTarget { Value = character });
                            CollisionTargetLimits.GetRefRW(target).ValueRW.Value--;
                        }
                    }
                }
            }
        }
    }
}