using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    [UpdateAfter(typeof(CharacterTargetSystem))]
    public partial struct CharacterMovementSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<Time>()) return;

            var deltaTime = SystemAPI.GetSingleton<Time>().DeltaTime;
            var speed = 3f;
            var rotationSpeed = 5f;
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);

            foreach (var (transform, externalVelocity, target, abilities, character) in SystemAPI
                         .Query<RefRW<LocalTransform>, RefRO<ExternalVelocity>, RefRO<Target>, DynamicBuffer<Ability>, RefRO<Character>>()
                         .WithNone<Dead>())
            {
                var moveVelocity = float3.zero;
                var direction = float3.zero;
                var targetEntity = target.ValueRO.Value;

                if (transforms.HasComponent(targetEntity) && characters.HasComponent(targetEntity))
                {
                    var targetConfig = characters[targetEntity].GetConfig();
                    var characterConfig = character.ValueRO.GetConfig();
                    var abilityRadius = abilities.Length > 0 ? abilities[0].GetConfig().Radius : 0;
                    var baseDistance = abilityRadius + characterConfig.ColliderRadius + targetConfig.ColliderRadius;
                    var stopDistance = abilities.Length > 0 && abilityRadius != 0 ? baseDistance : float.MaxValue;
                    var targetTransform = transforms[targetEntity];
                    direction = targetTransform.Position - transform.ValueRO.Position;
                    float distance = math.length(direction);
                    if (distance > stopDistance)
                    {
                        float3 moveDir = math.normalize(new float3(direction.x, 0f, direction.z));
                        moveVelocity = moveDir * speed;
                    }
                }

                var nextPosition = transform.ValueRO.Position + (moveVelocity + externalVelocity.ValueRO.Value) * deltaTime;
                if (nextPosition.y < 0f) nextPosition.y = 0f;
                transform.ValueRW.Position = nextPosition;

                var targetDir = new float3(direction.x, 0f, direction.z);
                if (math.lengthsq(targetDir) > 0.0001f)
                {
                    var targetRot = quaternion.LookRotationSafe(math.normalize(targetDir), math.up());
                    transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, targetRot, rotationSpeed * deltaTime);
                }
            }
        }
    }
}
