using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace vikwhite.ECS
{
    [BurstCompile]
    [UpdateInGroup(typeof(CollisionSystemGroup))]
    public partial struct AuraCollisionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            foreach (var aura in SystemAPI.Query<RefRW<Aura>>())
            {
                if (aura.ValueRO.TileLeft >= 0)
                    aura.ValueRW.TileLeft -= dt;
                else
                {
                    aura.ValueRW.TileLeft = aura.ValueRO.Interval;
                    var job = new AuraCollisionJob
                    {
                        Characters = SystemAPI.GetComponentLookup<Character>(true),
                        Enemies = SystemAPI.GetComponentLookup<Enemy>(true),
                        Deads = SystemAPI.GetComponentLookup<Dead>(true),
                        Auras = SystemAPI.GetComponentLookup<Aura>(true),
                        Providers = SystemAPI.GetComponentLookup<Provider>(true),
                        CollisionTargets = SystemAPI.GetBufferLookup<CollisionTarget>(),
                        CollisionTargetLimits = SystemAPI.GetComponentLookup<CollisionTargetLimit>(),
                    };
                    state.Dependency = job.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
                }
            }
        }
    }

    [BurstCompile]
    struct AuraCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<Character> Characters;
        [ReadOnly] public ComponentLookup<Enemy> Enemies;
        [ReadOnly] public ComponentLookup<Dead> Deads;
        [ReadOnly] public ComponentLookup<Aura> Auras;
        [ReadOnly] public ComponentLookup<Provider> Providers;
        public BufferLookup<CollisionTarget> CollisionTargets;
        public ComponentLookup<CollisionTargetLimit> CollisionTargetLimits;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (Deads.HasComponent(triggerEvent.EntityA) || Deads.HasComponent(triggerEvent.EntityB)) return;
            
            var a = triggerEvent.EntityA;
            var b = triggerEvent.EntityB;
            
            bool aIsProjectile = Auras.HasComponent(a);
            bool bIsProjectile = Auras.HasComponent(b);

            bool aIsCharacter = Characters.HasComponent(a);
            bool bIsCharacter = Characters.HasComponent(b);

            if ((aIsProjectile && bIsCharacter) || (bIsProjectile && aIsCharacter))
            {
                var character = aIsCharacter ? a : b;
                var projectile = aIsProjectile ? a : b;
            
                // не столкновение с источником ауры
                if (Providers.TryGetComponent(projectile, out var provider) && provider.Value != character)
                {
                    // столкновение с противником
                    if (Enemies.HasComponent(provider.Value) != Enemies.HasComponent(character))
                    {
                        CollisionTargets[projectile].Add(new CollisionTarget { Value = character });
                    }
                }
            }
        }
    }
}