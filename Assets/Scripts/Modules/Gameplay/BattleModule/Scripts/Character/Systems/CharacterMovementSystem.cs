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

            foreach (var (transform, externalVelocity, target, abilities, character, moveDistance, avoidance, entity) in SystemAPI
                         .Query<RefRW<LocalTransform>, RefRO<ExternalVelocity>, RefRO<Target>, DynamicBuffer<Ability>, RefRO<Character>, RefRO<MoveDistance>, RefRW<PathAvoidanceState>>()
                         .WithNone<Dead>()
                         .WithEntityAccess())
            {
                var isMovementLocked = SystemAPI.HasComponent<MovementLock>(entity);

                var moveVelocity = float3.zero;
                var direction = float3.zero;
                var lookDirection = float3.zero;
                var targetEntity = target.ValueRO.Value;

                if (transforms.HasComponent(targetEntity) && characters.HasComponent(targetEntity))
                {
                    var targetConfig = characters[targetEntity].GetConfig();
                    var characterConfig = character.ValueRO.GetConfig();
                    var abilityRadius = abilities.Length > 0 ? abilities[0].GetConfig().Radius : 0;
                    var baseDistance = abilityRadius + characterConfig.ColliderRadius + targetConfig.ColliderRadius;
                    var stopDistance = abilities.Length > 0 && abilityRadius != 0 ? baseDistance : float.MaxValue;
                    var targetTransform = transforms[targetEntity];
                    lookDirection = targetTransform.Position - transform.ValueRO.Position;
                    var desiredTargetPosition = targetTransform.Position;
                    if (!isMovementLocked && avoidance.ValueRO.HasWaypoint)
                    {
                        var waypointDistance = math.distance(transform.ValueRO.Position.xz, avoidance.ValueRO.Waypoint.xz);
                        var pathToTargetBlocked = IsPathBlocked(entity, targetEntity, transform.ValueRO.Position.xz, targetTransform.Position.xz, characterConfig.ColliderRadius, agents);
                        if (waypointDistance <= characterConfig.ColliderRadius * 1.5f || !pathToTargetBlocked)
                            ClearWaypoint(ref avoidance.ValueRW);
                        else
                            desiredTargetPosition = avoidance.ValueRO.Waypoint;
                    }

                    direction = desiredTargetPosition - transform.ValueRO.Position;
                    float distance = math.length(new float2(direction.x, direction.z));
                    if (!isMovementLocked && distance > stopDistance)
                    {
                        float3 desiredDir = math.normalize(new float3(direction.x, 0f, direction.z));
                        float3 moveDir = CalculatePathDirection(
                            entity,
                            targetEntity,
                            transform.ValueRO.Position,
                            targetTransform.Position,
                            desiredDir,
                            characterConfig.ColliderRadius,
                            speed,
                            deltaTime,
                            moveDistance.ValueRO.Value,
                            ref avoidance.ValueRW,
                            agents);
                        moveVelocity = moveDir * speed;
                    }
                    else if (!isMovementLocked)
                    {
                        ResetAvoidance(ref avoidance.ValueRW);
                    }
                }
                else if (!isMovementLocked)
                {
                    ResetAvoidance(ref avoidance.ValueRW);
                }

                var nextPosition = transform.ValueRO.Position + (moveVelocity + externalVelocity.ValueRO.Value) * deltaTime;
                if (nextPosition.y < 0f) nextPosition.y = 0f;
                transform.ValueRW.Position = nextPosition;

                var targetDir = new float3(lookDirection.x, 0f, lookDirection.z);
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
            float3 targetPosition,
            float3 desiredDirection,
            float radius,
            float speed,
            float deltaTime,
            float movedDistance,
            ref PathAvoidanceState avoidance,
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
            var hasBlockingObstacle = false;
            var blockingObstacle = Entity.Null;
            var blockingObstaclePosition = float2.zero;
            var blockingObstacleRadius = 0f;
            var blockingForwardDistance = float.MaxValue;
            var blockingCount = 0;
            var leftPressure = 0f;
            var rightPressure = 0f;

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

                blockingCount++;

                if (forwardDistance < blockingForwardDistance)
                {
                    hasBlockingObstacle = true;
                    blockingObstacle = other.Entity;
                    blockingObstaclePosition = other.Position.xz;
                    blockingObstacleRadius = other.Radius;
                    blockingForwardDistance = forwardDistance;
                }

                var sideSign = math.sign(Cross(desiredXZ, toOther));
                if (sideSign == 0f)
                    sideSign = entity.Index < other.Entity.Index ? 1f : -1f;
                if (sideSign == 0f)
                    sideSign = 1f;

                var pressure = (1f - math.saturate(sideDistanceAbs / math.max(corridor, 0.001f)))
                    * (1f - math.saturate(forwardDistance / math.max(lookAhead, 0.001f)));
                if (sideSign > 0f) rightPressure += pressure;
                else leftPressure += pressure;

                var tangent = new float2(-desiredXZ.y, desiredXZ.x) * -sideSign;
                var weight = 1f - math.saturate(forwardDistance / lookAhead);
                lateral += tangent * weight;
            }

            var preferredDirection = desiredXZ * goalWeight + separation * separationWeight + lateral * lateralWeight;
            if (math.lengthsq(preferredDirection) < 0.0001f)
                preferredDirection = desiredXZ;
            else
                preferredDirection = math.normalize(preferredDirection);

            var preferredSide = leftPressure <= rightPressure ? -1f : 1f;
            var narrowPassage = blockingCount >= 2;
            UpdateAvoidanceState(entity, position, targetPosition, desiredXZ, radius, speed, deltaTime, movedDistance, hasBlockingObstacle, narrowPassage, blockingObstacle, blockingObstaclePosition, blockingObstacleRadius, preferredSide, ref avoidance, agents);

            if (avoidance.DetourTime > 0f && hasBlockingObstacle && avoidance.Obstacle == blockingObstacle)
            {
                var detourDirection = CalculateDetourDirection(positionXZ, desiredXZ, blockingObstaclePosition, radius, blockingObstacleRadius, avoidance.SideSign, narrowPassage);
                if (math.lengthsq(detourDirection) > 0.0001f)
                {
                    preferredDirection = detourDirection;
                    avoidance.DetourTime = math.max(0f, avoidance.DetourTime - deltaTime);
                }
            }
            else if (!hasBlockingObstacle)
            {
                ResetAvoidance(ref avoidance);
            }

            var steering = ChooseBestDirection(entity, targetEntity, positionXZ, radius, desiredXZ, preferredDirection, speed, deltaTime, agents);
            var nextXZ = positionXZ + steering * speed * deltaTime;
            nextXZ = ResolvePenetration(entity, targetEntity, nextXZ, radius, desiredXZ, agents);
            var finalDir = nextXZ - positionXZ;
            if (math.lengthsq(finalDir) < 0.0001f) return float3.zero;

            finalDir = math.normalize(finalDir);
            return new float3(finalDir.x, 0f, finalDir.y);
        }

        private static void UpdateAvoidanceState(
            Entity entity,
            float3 position,
            float3 targetPosition,
            float2 desiredDirection,
            float radius,
            float speed,
            float deltaTime,
            float movedDistance,
            bool hasBlockingObstacle,
            bool narrowPassage,
            Entity blockingObstacle,
            float2 blockingObstaclePosition,
            float blockingObstacleRadius,
            float preferredSide,
            ref PathAvoidanceState avoidance,
            Unity.Collections.NativeArray<CharacterAgentData> agents)
        {
            const float stuckDistanceFactor = 0.25f;
            const float blockedThreshold = 0.1f;
            const float detourDuration = 0.9f;

            if (!hasBlockingObstacle)
            {
                avoidance.BlockedTime = math.max(0f, avoidance.BlockedTime - deltaTime * 2f);
                avoidance.DetourTime = math.max(0f, avoidance.DetourTime - deltaTime);
                if (avoidance.BlockedTime <= 0f && avoidance.DetourTime <= 0f)
                    ResetAvoidance(ref avoidance);
                return;
            }

            if (avoidance.Obstacle != blockingObstacle)
            {
                avoidance.Obstacle = blockingObstacle;
                avoidance.BlockedTime = 0f;
                avoidance.DetourTime = 0f;
                avoidance.SideSign = avoidance.SideSign == 0f
                    ? (entity.Index < blockingObstacle.Index ? 1f : -1f)
                    : avoidance.SideSign;
            }

            var expectedMove = speed * deltaTime;
            var isStuck = movedDistance < expectedMove * stuckDistanceFactor;
            avoidance.BlockedTime = isStuck
                ? avoidance.BlockedTime + deltaTime
                : math.max(0f, avoidance.BlockedTime - deltaTime * 1.5f);

            if (narrowPassage)
            {
                avoidance.SideSign = preferredSide;
                avoidance.DetourTime = math.max(avoidance.DetourTime, detourDuration);
                avoidance.BlockedTime = math.max(avoidance.BlockedTime, blockedThreshold);
                avoidance.Waypoint = BuildDetourWaypoint(position.xz, targetPosition.xz, blockingObstaclePosition, radius, blockingObstacleRadius, avoidance.SideSign, agents);
                avoidance.HasWaypoint = true;
                return;
            }

            if (avoidance.BlockedTime >= blockedThreshold)
            {
                if (avoidance.SideSign == 0f)
                    avoidance.SideSign = preferredSide != 0f ? preferredSide : (entity.Index < blockingObstacle.Index ? 1f : -1f);
                avoidance.DetourTime = math.max(avoidance.DetourTime, detourDuration);
                avoidance.Waypoint = BuildDetourWaypoint(position.xz, targetPosition.xz, blockingObstaclePosition, radius, blockingObstacleRadius, avoidance.SideSign, agents);
                avoidance.HasWaypoint = true;
            }
        }

        private static float3 BuildDetourWaypoint(
            float2 position,
            float2 targetPosition,
            float2 obstaclePosition,
            float radius,
            float obstacleRadius,
            float sideSign,
            Unity.Collections.NativeArray<CharacterAgentData> agents)
        {
            var toTarget = targetPosition - position;
            var forward = math.lengthsq(toTarget) > 0.0001f ? math.normalize(toTarget) : new float2(0f, 1f);
            var side = new float2(-forward.y, forward.x) * math.select(1f, sideSign, sideSign != 0f);
            var clearance = (radius + obstacleRadius) * 2.4f + 0.6f;
            var candidate = obstaclePosition + side * clearance + forward * (radius + obstacleRadius + 0.35f);

            for (int i = 0; i < agents.Length; i++)
            {
                var other = agents[i];
                var minDistance = radius + other.Radius + 0.1f;
                var delta = candidate - other.Position.xz;
                var distanceSq = math.lengthsq(delta);
                if (distanceSq < minDistance * minDistance)
                {
                    if (distanceSq < 0.0001f)
                        candidate += side * minDistance;
                    else
                        candidate = other.Position.xz + math.normalize(delta) * minDistance;
                }
            }

            return new float3(candidate.x, 0f, candidate.y);
        }

        private static bool IsPathBlocked(
            Entity entity,
            Entity targetEntity,
            float2 position,
            float2 targetPosition,
            float radius,
            Unity.Collections.NativeArray<CharacterAgentData> agents)
        {
            var toTarget = targetPosition - position;
            var distanceToTarget = math.length(toTarget);
            if (distanceToTarget < 0.0001f) return false;

            var direction = toTarget / distanceToTarget;
            for (int i = 0; i < agents.Length; i++)
            {
                var other = agents[i];
                if (other.Entity == entity || other.Entity == targetEntity) continue;

                var toOther = other.Position.xz - position;
                var forwardDistance = math.dot(toOther, direction);
                if (forwardDistance <= 0f || forwardDistance >= distanceToTarget) continue;

                var sideDistance = math.abs(Cross(direction, toOther));
                var corridor = radius + other.Radius + 0.1f;
                if (sideDistance < corridor) return true;
            }

            return false;
        }

        private static float2 CalculateDetourDirection(
            float2 position,
            float2 desiredDirection,
            float2 obstaclePosition,
            float radius,
            float obstacleRadius,
            float sideSign,
            bool narrowPassage)
        {
            var toObstacle = obstaclePosition - position;
            if (math.lengthsq(toObstacle) < 0.0001f)
                return new float2(-desiredDirection.y, desiredDirection.x) * math.select(1f, sideSign, sideSign != 0f);

            var tangent = new float2(-desiredDirection.y, desiredDirection.x) * math.select(1f, sideSign, sideSign != 0f);
            var clearance = radius + obstacleRadius + (narrowPassage ? 0.6f : 0.3f);
            var pushAway = math.normalize(position - obstaclePosition) * math.saturate(clearance / math.max(math.length(toObstacle), 0.001f));
            var tangentWeight = narrowPassage ? 2.2f : 1.35f;
            var goalWeight = narrowPassage ? 0.1f : 0.35f;
            var detour = tangent * tangentWeight + desiredDirection * goalWeight + pushAway;
            return math.lengthsq(detour) > 0.0001f ? math.normalize(detour) : tangent;
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

        private static void ResetAvoidance(ref PathAvoidanceState avoidance)
        {
            avoidance.Obstacle = Entity.Null;
            avoidance.SideSign = 0f;
            avoidance.BlockedTime = 0f;
            avoidance.DetourTime = 0f;
            avoidance.Waypoint = float3.zero;
            avoidance.HasWaypoint = false;
        }

        private static void ClearWaypoint(ref PathAvoidanceState avoidance)
        {
            avoidance.Waypoint = float3.zero;
            avoidance.HasWaypoint = false;
        }

        private struct CharacterAgentData
        {
            public Entity Entity;
            public float3 Position;
            public float Radius;
        }
    }
}
