using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(MovementSystemGroup))]
    public partial struct OrbitMovementSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            foreach (var (transform, movement, speed, target) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<OrbitMovement>, RefRO<Speed>, RefRO<Target>>())
            {
                var phase = movement.ValueRO.Phase + dt * speed.ValueRO.Value;
                movement.ValueRW.Phase = phase;
                var newRelativePosition = new float3(Mathf.Cos(phase) * movement.ValueRO.Radius, 0.5f, Mathf.Sin(phase) * movement.ValueRO.Radius);
                var targetPosition = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.Value).Position;
                transform.ValueRW.Position = newRelativePosition + targetPosition;
            }
        }
    }
}