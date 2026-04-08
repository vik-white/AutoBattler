using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct TimeSystem :ISystem
    {
        public void OnCreate(ref SystemState state) {
            state.EntityManager.AddComponent<Time>(state.EntityManager.CreateEntity());
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var time in SystemAPI.Query<RefRW<Time>>()) {
                time.ValueRW.DeltaTime = time.ValueRO.IsPaused ? 0 : SystemAPI.Time.DeltaTime;
            }
        }
    }
}