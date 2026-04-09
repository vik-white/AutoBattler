using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using vikwhite.ECS;

namespace vikwhite
{
    [BurstCompile]
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct DestroyOutsideSceneSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAny<DestroyOutsideScene>().WithEntityAccess()) {
                var position = transform.ValueRO.Position;
                if (position.x > 20 || position.x < -20 || position.y > 50 || position.y < -1 || position.z > 20 || position.z < -20) 
                    ecb.DestroyEntity(entity);
            }
        }
    }
}