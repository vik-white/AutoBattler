using Unity.Entities;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct FollowProviderSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            foreach (var (transform, target) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Target>>().WithAny<FollowingProvider>()) 
            {
                transform.ValueRW.Position = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.Value).Position;
            }
        }
    }
}