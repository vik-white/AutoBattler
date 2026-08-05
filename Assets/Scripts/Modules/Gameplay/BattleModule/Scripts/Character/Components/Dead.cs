using Unity.Entities;
using Unity.Physics;

namespace vikwhite.ECS
{
    public struct Dead: IComponentData
    {
        public PhysicsCollider Collider;
        public bool AnimationCompleted;
    }
}
