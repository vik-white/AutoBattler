using Unity.Entities;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct PreviousPositionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, previous) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PreviousPosition>>()) {
                previous.ValueRW.Value = transform.ValueRO.Position;
            }
        }
    }
}