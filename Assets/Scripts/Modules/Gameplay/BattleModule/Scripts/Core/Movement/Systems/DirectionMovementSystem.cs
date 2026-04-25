using Unity.Entities;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(MovementSystemGroup))]
    public partial struct DirectionMovementSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            foreach (var (transform, movement, speed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<DirectionMovement>, RefRO<Speed>>()) {
                transform.ValueRW.Position += movement.ValueRO.Direction * speed.ValueRO.Value * dt;
            }
        }
    }
}
