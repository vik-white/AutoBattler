using Unity.Entities;
using Unity.Physics;

namespace vikwhite.ECS
{
    public static class EntityCommandBufferExtensions
    {
        public static void DestroyEntityAndPhysics(this EntityCommandBuffer ecb, EntityManager entityManager, Entity entity)
        {
            if (entityManager.HasComponent<PhysicsCollider>(entity))
            {
                var physicsCollider = entityManager.GetComponentData<PhysicsCollider>(entity);
                if (physicsCollider.Value.IsCreated)
                    physicsCollider.Value.Dispose();
            }

            if (entityManager.HasComponent<JumpPhysicsState>(entity))
            {
                var jumpPhysicsState = entityManager.GetComponentData<JumpPhysicsState>(entity);
                if (jumpPhysicsState.Collider.Value.IsCreated)
                    jumpPhysicsState.Collider.Value.Dispose();
            }

            ecb.DestroyEntity(entity);
        }
    }
}
