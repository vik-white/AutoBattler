using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct ExternalVelocitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            foreach (var velocity in SystemAPI.Query<RefRW<ExternalVelocity>>()) 
            {
                velocity.ValueRW.Value *= 0.9f;
            }
        }
    }
}