using Unity.Burst;
using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    [BurstCompile]
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct CleanupCollisionTargetsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            foreach (var collisionTargets in SystemAPI.Query<DynamicBuffer<CollisionTarget>>()) 
            {
                collisionTargets.Clear();
            }
        }
    }
}