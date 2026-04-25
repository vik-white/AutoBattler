using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct PauseSystem :ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var time = SystemAPI.GetSingletonRW<Time>();
            //foreach (var _ in SystemAPI.Query<RefRO<LevelUpEvent>>()) time.ValueRW.IsPaused = true;
            //foreach (var _ in SystemAPI.Query<RefRO<UpgradeAbilityEvent>>()) time.ValueRW.IsPaused = false;
        }
    }
}