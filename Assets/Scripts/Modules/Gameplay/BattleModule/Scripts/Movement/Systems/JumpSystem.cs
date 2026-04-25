using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(MovementSystemGroup))]
    public partial struct JumpSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            float deltaTime = SystemAPI.GetSingleton<Time>().DeltaTime;
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);

            foreach (var (transform, jump, character, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Jump>, RefRO<Character>>().WithNone<Dead>().WithEntityAccess())
            {
                Entity target = jump.ValueRO.Value;
                if (!transforms.HasComponent(target) || !characters.HasComponent(target))
                {
                    ecb.RemoveComponent<Jump>(entity);
                    continue;
                }

                float3 currentPosition = transform.ValueRO.Position;
                float3 targetPosition = transforms[target].Position;
                float distanceToTarget = math.distance(currentPosition, targetPosition);

                var characterConfig = character.ValueRO.GetConfig();
                var targetConfig = characters[target].GetConfig();
                float finishDistance = characterConfig.ColliderRadius + targetConfig.ColliderRadius + 0.5f;

                if (distanceToTarget <= finishDistance)
                {
                    ecb.RemoveComponent<Jump>(entity);
                    continue;
                }

                float progress = math.saturate(jump.ValueRO.Progress + deltaTime / jump.ValueRO.Duration);
                jump.ValueRW.Progress = progress;

                float3 groundPosition = math.lerp(jump.ValueRO.StartPosition, targetPosition, progress);
                groundPosition.y += math.sin(progress * math.PI) * jump.ValueRO.Height;
                transform.ValueRW.Position = groundPosition;

                float3 lookDirection = targetPosition - transform.ValueRW.Position;
                lookDirection.y = 0f;
                if (math.lengthsq(lookDirection) > 0.0001f)
                    transform.ValueRW.Rotation = quaternion.LookRotationSafe(math.normalize(lookDirection), math.up());

                if (progress >= 1f)
                    ecb.RemoveComponent<Jump>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
