using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    public partial struct PreviousPositionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, previous, distance) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PreviousPosition>, RefRW<MoveDistance>>()) {
                float3 currentPos = transform.ValueRO.Position;
                float3 prevPos = previous.ValueRO.Value;
                float dist = math.distance(currentPos, prevPos);
                distance.ValueRW.Value = dist;
                previous.ValueRW.Value = currentPos;
            }
        }
    }
}