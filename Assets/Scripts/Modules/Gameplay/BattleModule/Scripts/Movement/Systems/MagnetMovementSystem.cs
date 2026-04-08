using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(MovementSystemGroup))]
    public partial struct MagnetMovementSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            foreach (var (transform, movement, speed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MagnetMovement>, RefRO<Speed>>()) {
                if(!SystemAPI.Exists(movement.ValueRO.Entity)) continue;
                var targetPosition = transforms[movement.ValueRO.Entity].Position;
                transform.ValueRW.Position += math.normalize(targetPosition - transform.ValueRO.Position) * speed.ValueRO.Value * dt;
            }
        }
    }
}