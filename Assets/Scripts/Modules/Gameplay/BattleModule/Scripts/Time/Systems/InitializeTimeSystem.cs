using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public partial struct InitializeTimeSystem :ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false;
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var time in SystemAPI.Query<RefRW<Time>>()) {
                time.ValueRW.TotalTime = 0;
            }
            state.Enabled = false;
        }
    }
}