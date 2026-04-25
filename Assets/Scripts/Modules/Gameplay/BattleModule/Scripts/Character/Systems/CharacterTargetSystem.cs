using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct CharacterTargetSystem : ISystem
    {
        private EntityQuery _characterQuery;

        public void OnCreate(ref SystemState state)
        {
            _characterQuery = state.GetEntityQuery(ComponentType.ReadOnly<Character>(), ComponentType.ReadOnly<LocalTransform>());
        }
        
        public void OnUpdate(ref SystemState state) {
            if (!SystemAPI.HasSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()) return;

            var characters = _characterQuery.ToEntityArray(Allocator.TempJob);
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var job = new CharacterTargetJob
            {
                Characters = characters,
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
                EnemyLookup = SystemAPI.GetComponentLookup<Enemy>(true),
                DeadLookup = SystemAPI.GetComponentLookup<Dead>(true),
                TargetLookup = SystemAPI.GetComponentLookup<Target>(true),
                AggroLookup = SystemAPI.GetComponentLookup<Aggro>(true),
                Ecb = ecb
            };
            job.ScheduleParallel(SystemAPI.QueryBuilder().WithAll<Character>().WithNone<Dead>().Build());
            characters.Dispose(state.Dependency);
        }
    }
    
    [BurstCompile]
    [WithNone(typeof(Dead))]
    public partial struct CharacterTargetJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> Characters;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        [ReadOnly] public ComponentLookup<Enemy> EnemyLookup;
        [ReadOnly] public ComponentLookup<Dead> DeadLookup;
        [ReadOnly] public ComponentLookup<Target> TargetLookup;
        [ReadOnly] public ComponentLookup<Aggro> AggroLookup;
        public EntityCommandBuffer.ParallelWriter Ecb;

        [BurstCompile]
        private void Execute(Entity entity, [EntityIndexInQuery] int sortKey)
        {
            var position = TransformLookup[entity].Position;
            var isEnemy = EnemyLookup.HasComponent(entity);
            var minDistanceSq = float.MaxValue;
            var newTarget = Entity.Null;
            var oldTarget = TargetLookup.HasComponent(entity) ? TargetLookup[entity].Value : Entity.Null;

            if (!AggroLookup.HasComponent(entity))
            {
                for (int i = 0; i < Characters.Length; i++)
                {
                    Entity otherEntity = Characters[i];
                    bool otherIsDead = DeadLookup.HasComponent(otherEntity);
                    if (otherEntity == entity || otherIsDead) continue;
                    bool otherIsEnemy = EnemyLookup.HasComponent(otherEntity);
                    if (isEnemy == otherIsEnemy) continue;
                    float3 otherPosition = TransformLookup[otherEntity].Position;
                    float distSq = math.distancesq(position, otherPosition);
                    if (distSq < minDistanceSq)
                    {
                        minDistanceSq = distSq;
                        newTarget = otherEntity;
                        if(oldTarget == newTarget) break;
                    }
                }
            }
            else
            {
                newTarget = AggroLookup[entity].Provider;
            }
            
            if (newTarget != Entity.Null)
            {
                if (!TargetLookup.HasComponent(entity))
                    Ecb.AddComponent(sortKey, entity, new Target { Value = newTarget });
                else
                    Ecb.SetComponent(sortKey, entity, new Target { Value = newTarget });
            }
            else if (TargetLookup.HasComponent(entity))
            {
                Ecb.RemoveComponent<Target>(sortKey, entity);
            }
        }
    }
}
