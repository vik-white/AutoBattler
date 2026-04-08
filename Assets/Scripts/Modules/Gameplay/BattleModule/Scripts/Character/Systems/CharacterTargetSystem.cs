using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct CharacterTargetSystem : ISystem
    {
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
        }
    }
}