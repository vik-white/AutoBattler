using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
            var job = new CharacterMovementJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                Speed = 3f,
                RotationSpeed = 5f,
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true)
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    [BurstCompile]
    public partial struct CharacterMovementJob : IJobEntity
    {
        public float DeltaTime;
        public float Speed;
        public float RotationSpeed;
        [ReadOnly] 
        [NativeDisableContainerSafetyRestriction] 
        public ComponentLookup<LocalTransform> TransformLookup;

        [BurstCompile]
        private void Execute(ref PhysicsVelocity physicsVelocity, ref LocalTransform transform, in Target target, in DynamicBuffer<Ability> abilities)
        {
            var stopDistance = abilities.Length > 0 ? abilities[0].Config.Radius : float.MaxValue;
            
            Entity targetEntity = target.Value;
            if (!TransformLookup.HasComponent(targetEntity)) 
            {
                physicsVelocity.Linear = float3.zero;
                return;
            }

            LocalTransform targetTransform = TransformLookup[targetEntity];
            float3 direction = targetTransform.Position - transform.Position;
            float distance = math.length(direction);
            if (distance > stopDistance)
            {
                float3 moveDir = math.normalize(new float3(direction.x, 0f, direction.z));
                physicsVelocity.Linear = moveDir * Speed;
            }
            else
            {
                physicsVelocity.Linear = float3.zero;
            }

            if (distance > 0.001f)
            {
                float3 targetDir = math.normalize(new float3(direction.x, 0f, direction.z));
                quaternion targetRot = quaternion.LookRotationSafe(targetDir, math.up());
                transform.Rotation = math.slerp(transform.Rotation, targetRot, RotationSpeed * DeltaTime);
                physicsVelocity.Angular = float3.zero;
            }
        }
    }
}