using Unity.Entities;
using Unity.Physics;

namespace vikwhite.ECS
{
    public struct JumpPhysicsState : IComponentData
    {
        public PhysicsMass Mass;
        public PhysicsVelocity Velocity;
        public PhysicsCollider Collider;
    }
}