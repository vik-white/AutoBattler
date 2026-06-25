using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(TimeSystemGroup), OrderFirst = true)]
    public partial struct InitializeTimeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var time in SystemAPI.Query<RefRW<Time>>())
            {
                TimeSystem.Reset(ref time.ValueRW);
            }
            state.Enabled = false;
        }
    }
}
