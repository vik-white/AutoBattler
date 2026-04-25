using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace vikwhite.ECS
{
    [BurstCompile]
    [UpdateInGroup(typeof(CollisionSystemGroup))]
    public partial struct ProjectileCollisionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<SimulationSingleton>()) return;

            var job = new ProjectileCollisionJob
            {
                Characters = SystemAPI.GetComponentLookup<Character>(true),
                Enemies = SystemAPI.GetComponentLookup<Enemy>(true),
                Deads = SystemAPI.GetComponentLookup<Dead>(true),
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
    struct ProjectileCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<Character> Characters;
        [ReadOnly] public ComponentLookup<Enemy> Enemies;
        [ReadOnly] public ComponentLookup<Dead> Deads;
        [ReadOnly] public ComponentLookup<Projectile> Projectiles;
        [ReadOnly] public ComponentLookup<Provider> Providers;
        public BufferLookup<CollisionTarget> CollisionTargets;
        public BufferLookup<CollisionBuffer> CollisionBuffers;
        public ComponentLookup<CollisionTargetLimit> CollisionTargetLimits;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (Deads.HasComponent(triggerEvent.EntityA) || Deads.HasComponent(triggerEvent.EntityB)) return;
            
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
                // доступный лимит столкновений для проджектайла
                if (!isHaveTargetLimit || CollisionTargetLimits[projectile].Value > 0)
                {
                    // не столкновение с источником проджектайла
                    if (Providers.TryGetComponent(projectile, out var provider) && provider.Value != character)
                    {
                        // столкновение с противником
                        if (Enemies.HasComponent(provider.Value) != Enemies.HasComponent(character))
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
}
