using Unity.Entities;

namespace vikwhite.ECS
{
    public static class EntityCommandBufferExtensions
    {
        public static Entity CreateEntity<T>(this EntityCommandBuffer ecb, in T component) where T : unmanaged, IComponentData {
            var entity = ecb.CreateEntity();
            ecb.AddComponent(entity, component);
            return entity;
        }
        
        public static Entity CreateFrameEntity<T>(this EntityCommandBuffer ecb, in T component) where T : unmanaged, IComponentData {
            var entity = ecb.CreateEntity();
            ecb.AddComponent(entity, component);
            ecb.AddComponent<Destroy>(entity);
            return entity;
        }
    }
}