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
                Projectiles = SystemAPI.GetComponentLookup<Projectile>(true),
                Providers = SystemAPI.GetComponentLookup<Provider>(true),
                CollisionTargets = SystemAPI.GetBufferLookup<CollisionTarget>(),
                CollisionBuffers = SystemAPI.GetBufferLookup<CollisionBuffer>(),
                CollisionTargetLimits = SystemAPI.GetComponentLookup<CollisionTargetLimit>(),
            };
            state.Dependency = job.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }
    }

    [BurstCompile]
    [WithNone(typeof(Dead))]
    struct CharacterCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<Character> Characters;
        [ReadOnly] public ComponentLookup<Projectile> Projectiles;
        [ReadOnly] public ComponentLookup<Provider> Providers;
        public BufferLookup<CollisionTarget> CollisionTargets;
        public BufferLookup<CollisionBuffer> CollisionBuffers;
        public ComponentLookup<CollisionTargetLimit> CollisionTargetLimits;

        public void Execute(TriggerEvent triggerEvent)
        {
            
            var a = triggerEvent.EntityA;
            var b = triggerEvent.EntityB;
            
            bool aIsProjectile = Projectiles.HasComponent(a);
            bool bIsProjectile = Projectiles.HasComponent(b);

            bool aIsCharacter = Characters.HasComponent(a);
            bool bIsCharacter = Characters.HasComponent(b);

            if ((aIsProjectile && bIsCharacter) || (bIsProjectile && aIsCharacter))
            {
                var character = aIsCharacter ? a : b;
                var projectile = aIsProjectile ? a : b;
            
                var isHaveTargetLimit = CollisionTargetLimits.HasComponent(projectile);
                if (!isHaveTargetLimit || CollisionTargetLimits[projectile].Value > 0)
                {
                    if (Providers.TryGetComponent(projectile, out var provider) && provider.Value != character)
                    {
                        var collisionBuffer = CollisionBuffers[projectile];
                        var isContains = false;
                        foreach (var collision in collisionBuffer) {
                            if (collision.Value == character) {
                                isContains =  true;
                                break;
                            }
                        }
                        if (!isContains) {
                            collisionBuffer.Clear();
                            collisionBuffer.Add(new CollisionBuffer { Value = character });
                            CollisionTargets[projectile].Add(new CollisionTarget { Value = character });
                            if(isHaveTargetLimit) CollisionTargetLimits.GetRefRW(projectile).ValueRW.Value--;
                        }
                    }
                }
            }
        }
    }
}