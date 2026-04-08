using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    [UpdateAfter(typeof(CharacterTargetSystem))]
    public partial struct CharacterMovementSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            float speed = 3f;
            float rotationSpeed = 5f;
            float stopDistance = 1.5f;
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (physicsVelocity, transform, target) in SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<LocalTransform>, RefRO<Target>>().WithAny<Character>())
            {
                if (!SystemAPI.Exists(target.ValueRO.Value)) continue;

                var targetTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.Value);
                float3 direction = targetTransform.Position - transform.ValueRW.Position;
                float distance = math.length(direction);

                if (distance > stopDistance)
                {
                    float3 moveDir = math.normalize(new float3(direction.x, 0f, direction.z));
                    physicsVelocity.ValueRW.Linear = moveDir * speed;
                }
                else
                {
                    physicsVelocity.ValueRW.Linear = float3.zero;
                }

                if (distance > 0.001f)
                {
                    float3 targetDir = math.normalize(new float3(direction.x, 0f, direction.z));
                    quaternion targetRot = quaternion.LookRotationSafe(targetDir, math.up());
                    transform.ValueRW.Rotation = math.slerp(transform.ValueRW.Rotation, targetRot, rotationSpeed * deltaTime);
                    physicsVelocity.ValueRW.Angular = float3.zero;
                }
            }
        }
    }
}