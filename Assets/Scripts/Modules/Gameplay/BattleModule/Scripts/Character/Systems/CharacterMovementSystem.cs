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

            var characterQuery = SystemAPI.QueryBuilder().WithAll<Character, LocalTransform>().WithNone<Dead>().Build();
            var entities = characterQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            var transformsSnapshot = characterQuery.ToComponentDataArray<LocalTransform>(Unity.Collections.Allocator.Temp);
            var charactersSnapshot = characterQuery.ToComponentDataArray<Character>(Unity.Collections.Allocator.Temp);
            var agents = new Unity.Collections.NativeArray<CharacterAgentData>(entities.Length, Unity.Collections.Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                agents[i] = new CharacterAgentData
                {
                    Entity = entities[i],
                    Position = transformsSnapshot[i].Position,
                    Radius = charactersSnapshot[i].GetConfig().ColliderRadius
                };
            }

            var deltaTime = SystemAPI.GetSingleton<Time>().DeltaTime;
            var speed = 3f;
            var rotationSpeed = 5f;
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);

            foreach (var (transform, externalVelocity, target, abilities, character, entity) in SystemAPI
                         .Query<RefRW<LocalTransform>, RefRO<ExternalVelocity>, RefRO<Target>, DynamicBuffer<Ability>, RefRO<Character>>()
                         .WithNone<Dead>()
                         .WithEntityAccess())
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
                    float distance = math.length(new float2(direction.x, direction.z));
                    if (distance > stopDistance)
                    {
                        float3 desiredDir = math.normalize(new float3(direction.x, 0f, direction.z));
                        float3 moveDir = CalculatePathDirection(
                            entity,
                            targetEntity,
                            transform.ValueRO.Position,
                            desiredDir,
                            characterConfig.ColliderRadius,
                            speed,
                            deltaTime,
                            agents);
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

            agents.Dispose();
            charactersSnapshot.Dispose();
            transformsSnapshot.Dispose();
            entities.Dispose();
        }

        private static float3 CalculatePathDirection(
            Entity entity,
            Entity targetEntity,
            float3 position,
            float3 desiredDirection,
            float radius,
            float speed,
            float deltaTime,
            Unity.Collections.NativeArray<CharacterAgentData> agents)
        {
            if (math.lengthsq(desiredDirection) < 0.0001f) return float3.zero;

            const float separationBuffer = 0.15f;
            const float sideBuffer = 0.25f;
            const float goalWeight = 1.15f;
            const float separationWeight = 2.2f;
            const float lateralWeight = 1.4f;

            var desiredXZ = math.normalize(new float2(desiredDirection.x, desiredDirection.z));
            var separation = float2.zero;
            var lateral = float2.zero;
            var positionXZ = position.xz;
            var lookAhead = math.max(radius * 4f, speed * math.max(deltaTime, 0.1f) * 4f);

            for (int i = 0; i < agents.Length; i++)
            {
                var other = agents[i];
                if (other.Entity == entity || other.Entity == targetEntity) continue;

                var toOther = other.Position.xz - positionXZ;
                var distanceSq = math.lengthsq(toOther);
                if (distanceSq < 0.0001f)
                {
                    var emergencySide = new float2(-desiredXZ.y, desiredXZ.x);
                    separation -= emergencySide;
                    continue;
                }

                var distance = math.sqrt(distanceSq);
                var minDistance = radius + other.Radius;
                var avoidanceDistance = minDistance + separationBuffer;

                if (distance < avoidanceDistance)
                {
                    var pushDir = (positionXZ - other.Position.xz) / distance;
                    separation += pushDir * ((avoidanceDistance - distance) / math.max(avoidanceDistance, 0.001f));
                }

                var forwardDistance = math.dot(toOther, desiredXZ);
                if (forwardDistance <= 0f || forwardDistance > lookAhead) continue;

                var sideDistanceAbs = math.abs(Cross(desiredXZ, toOther));
                var corridor = minDistance + sideBuffer;
                if (sideDistanceAbs > corridor) continue;

                var sideSign = math.sign(Cross(desiredXZ, toOther));
                if (sideSign == 0f)
                    sideSign = entity.Index < other.Entity.Index ? 1f : -1f;
                if (sideSign == 0f)
                    sideSign = 1f;

                var tangent = new float2(-desiredXZ.y, desiredXZ.x) * -sideSign;
                var weight = 1f - math.saturate(forwardDistance / lookAhead);
                lateral += tangent * weight;
            }

            var preferredDirection = desiredXZ * goalWeight + separation * separationWeight + lateral * lateralWeight;
            if (math.lengthsq(preferredDirection) < 0.0001f)
                preferredDirection = desiredXZ;
            else
                preferredDirection = math.normalize(preferredDirection);

            var steering = ChooseBestDirection(entity, targetEntity, positionXZ, radius, desiredXZ, preferredDirection, speed, deltaTime, agents);
            var nextXZ = positionXZ + steering * speed * deltaTime;
            nextXZ = ResolvePenetration(entity, targetEntity, nextXZ, radius, desiredXZ, agents);
            var finalDir = nextXZ - positionXZ;
            if (math.lengthsq(finalDir) < 0.0001f) return float3.zero;

            finalDir = math.normalize(finalDir);
            return new float3(finalDir.x, 0f, finalDir.y);
        }

        private static float2 ChooseBestDirection(
            Entity entity,
            Entity targetEntity,
            float2 position,
            float radius,
            float2 desiredDirection,
            float2 preferredDirection,
            float speed,
            float deltaTime,
            Unity.Collections.NativeArray<CharacterAgentData> agents)
        {
            float2 bestDirection = desiredDirection;
            float bestScore = float.MinValue;

            float2[] directions =
            {
                preferredDirection,
                desiredDirection,
                Rotate(preferredDirection, math.radians(20f)),
                Rotate(preferredDirection, math.radians(-20f)),
                Rotate(preferredDirection, math.radians(40f)),
                Rotate(preferredDirection, math.radians(-40f)),
                Rotate(preferredDirection, math.radians(65f)),
                Rotate(preferredDirection, math.radians(-65f)),
                Rotate(preferredDirection, math.radians(90f)),
                Rotate(preferredDirection, math.radians(-90f))
            };

            for (int i = 0; i < directions.Length; i++)
            {
                var candidate = directions[i];
                if (math.lengthsq(candidate) < 0.0001f) continue;

                candidate = math.normalize(candidate);
                var score = ScoreDirection(entity, targetEntity, position, radius, desiredDirection, candidate, speed, deltaTime, agents);
                if (score <= bestScore) continue;

                bestScore = score;
                bestDirection = candidate;
            }

            return bestDirection;
        }

        private static float ScoreDirection(
            Entity entity,
            Entity targetEntity,
            float2 position,
            float radius,
            float2 desiredDirection,
            float2 candidate,
            float speed,
            float deltaTime,
            Unity.Collections.NativeArray<CharacterAgentData> agents)
        {
            const float skin = 0.05f;

            var lookAhead = math.max(radius * 4f, speed * math.max(deltaTime, 0.1f) * 5f);
            var nextPosition = position + candidate * speed * deltaTime;
            var score = math.dot(candidate, desiredDirection) * 4f;

            for (int i = 0; i < agents.Length; i++)
            {
                var other = agents[i];
                if (other.Entity == entity || other.Entity == targetEntity) continue;

                var toOther = other.Position.xz - position;
                var forwardDistance = math.dot(toOther, candidate);
                var corridorRadius = radius + other.Radius + skin;

                if (forwardDistance >= 0f && forwardDistance <= lookAhead)
                {
                    var sideDistance = math.abs(Cross(candidate, toOther));
                    if (sideDistance < corridorRadius)
                    {
                        var closeness = 1f - math.saturate(sideDistance / math.max(corridorRadius, 0.001f));
                        var forwardWeight = 1f - math.saturate(forwardDistance / math.max(lookAhead, 0.001f));
                        score -= 12f * closeness * forwardWeight;
                    }
                }

                var distanceToNext = math.distance(nextPosition, other.Position.xz);
                if (distanceToNext < corridorRadius)
                {
                    var penetration = 1f - math.saturate(distanceToNext / math.max(corridorRadius, 0.001f));
                    score -= 20f * penetration;
                }
            }

            return score;
        }

        private static float2 ResolvePenetration(
            Entity entity,
            Entity targetEntity,
            float2 nextPosition,
            float radius,
            float2 desiredDirection,
            Unity.Collections.NativeArray<CharacterAgentData> agents)
        {
            const float skin = 0.02f;

            for (int i = 0; i < agents.Length; i++)
            {
                var other = agents[i];
                if (other.Entity == entity || other.Entity == targetEntity) continue;

                var delta = nextPosition - other.Position.xz;
                var distanceSq = math.lengthsq(delta);
                var minDistance = radius + other.Radius + skin;

                if (distanceSq >= minDistance * minDistance) continue;

                if (distanceSq < 0.0001f)
                {
                    var side = new float2(-desiredDirection.y, desiredDirection.x);
                    if (math.lengthsq(side) < 0.0001f)
                        side = new float2(1f, 0f);
                    nextPosition = other.Position.xz + math.normalize(side) * minDistance;
                    continue;
                }

                var distance = math.sqrt(distanceSq);
                nextPosition = other.Position.xz + (delta / distance) * minDistance;
            }

            return nextPosition;
        }

        private static float Cross(float2 a, float2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        private static float2 Rotate(float2 direction, float angle)
        {
            var sin = math.sin(angle);
            var cos = math.cos(angle);
            return new float2(
                direction.x * cos - direction.y * sin,
                direction.x * sin + direction.y * cos);
        }

        private struct CharacterAgentData
        {
            public Entity Entity;
            public float3 Position;
            public float Radius;
        }
    }
}
