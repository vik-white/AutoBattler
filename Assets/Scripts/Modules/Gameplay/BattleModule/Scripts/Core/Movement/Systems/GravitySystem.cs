using Unity.Entities;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(MovementSystemGroup))]
    [UpdateAfter(typeof(JumpSystem))]
    public partial struct GravitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<Time>()) return;

            var deltaTime = SystemAPI.GetSingleton<Time>().DeltaTime;
            const float gravity = 20f;

            foreach (var (transform, velocity) in SystemAPI
                         .Query<RefRW<LocalTransform>, RefRW<ExternalVelocity>>()
                         .WithAll<Character>()
                         .WithNone<Dead, Jump>())
            {
                if (transform.ValueRO.Position.y > 0f)
                {
                    velocity.ValueRW.Value.y -= gravity * deltaTime;
                }
                else if (velocity.ValueRO.Value.y < 0f)
                {
                    velocity.ValueRW.Value.y = 0f;
                }
            }
        }
    }
}
