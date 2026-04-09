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
        /*private EntityQuery _characterQuery;

        public void OnCreate(ref SystemState state)
        {
            _characterQuery = state.GetEntityQuery(ComponentType.ReadOnly<Character>(), ComponentType.ReadOnly<LocalTransform>());
        }*/
        
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var allEntities = new NativeList<Entity>(Allocator.Temp);
            var allPositions = new NativeList<float3>(Allocator.Temp);
            
            foreach (var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Character>().WithEntityAccess()) 
            {
                allEntities.Add(entity);
                allPositions.Add(transform.ValueRO.Position);
            }
            
            foreach (var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Character>().WithEntityAccess()) 
            {
                float3 myPosition = transform.ValueRO.Position;
                float minDistance = float.MaxValue;
                Entity nearest = Entity.Null;
                
                for (int i = 0; i < allEntities.Length; i++) 
                {
                    if (allEntities[i] == entity) continue;
                    if (SystemAPI.HasComponent<Enemy>(allEntities[i]) == SystemAPI.HasComponent<Enemy>(entity)) continue;
                    float distance = math.distance(myPosition, allPositions[i]);
                    if (distance < minDistance) 
                    {
                        minDistance = distance;
                        nearest = allEntities[i];
                    }
                }

                if (nearest != Entity.Null)
                {
                    if (!SystemAPI.HasComponent<Target>(entity))
                        ecb.AddComponent(entity, new Target { Value = nearest });
                    else
                        ecb.SetComponent(entity, new Target { Value = nearest });
                }
                else
                {
                    if (SystemAPI.HasComponent<Target>(entity))
                        ecb.RemoveComponent<Target>(entity);
                }
            }
            
            allEntities.Dispose();
            allPositions.Dispose();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            
            
            /*var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var characters = _characterQuery.ToEntityArray(Allocator.TempJob);
            var job = new CharacterTargetJob
            {
                Characters = characters,
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
                EnemyLookup = SystemAPI.GetComponentLookup<Enemy>(true),
                TargetLookup = SystemAPI.GetComponentLookup<Target>(true),
                Ecb = ecb
            };
            state.Dependency = job.Schedule(state.Dependency);
            state.Dependency.Complete();
            characters.Dispose(state.Dependency);*/
        }
    }
    
    [BurstCompile]
    public partial struct CharacterTargetJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> Characters;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        [ReadOnly] public ComponentLookup<Enemy> EnemyLookup;
        [ReadOnly] public ComponentLookup<Target> TargetLookup;
        public EntityCommandBuffer.ParallelWriter Ecb;

        [BurstCompile]
        private void Execute(Entity entity, [ReadOnly] in LocalTransform transform, [EntityIndexInQuery] int sortKey)
        {
            float3 position = transform.Position;
            bool isEnemy = EnemyLookup.HasComponent(entity);
            float minDistanceSq = float.MaxValue;
            Entity nearest = Entity.Null;
            
            for (int i = 0; i < Characters.Length; i++)
            {
                Entity otherEntity = Characters[i];
                if (otherEntity == entity) continue;
                bool otherIsEnemy = EnemyLookup.HasComponent(otherEntity);
                if (isEnemy == otherIsEnemy) continue;
                float3 otherPosition = TransformLookup[otherEntity].Position;
                float distSq = math.distancesq(position, otherPosition);
                if (distSq < minDistanceSq)
                {
                    minDistanceSq = distSq;
                    nearest = otherEntity;
                }
            }
            
            if (nearest != Entity.Null)
            {
                if (!TargetLookup.HasComponent(entity))
                    Ecb.AddComponent(sortKey, entity, new Target { Value = nearest });
                else
                    Ecb.SetComponent(sortKey, entity, new Target { Value = nearest });
            }
            else if (TargetLookup.HasComponent(entity))
            {
                Ecb.RemoveComponent<Target>(sortKey, entity);
            }
        }
    }
}