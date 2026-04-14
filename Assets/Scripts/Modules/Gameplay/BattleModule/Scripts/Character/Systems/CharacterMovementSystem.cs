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
                DeltaTime = SystemAPI.GetSingleton<Time>().DeltaTime,
                Speed = 3f,
                RotationSpeed = 5f,
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
                Characters = SystemAPI.GetComponentLookup<Character>(true),
                CharacterConfigs = SystemAPI.GetSingletonBuffer<CharacterConfig>(true)
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    [BurstCompile]
    [WithNone(typeof(Dead))]
    public partial struct CharacterMovementJob : IJobEntity
    {
        public float DeltaTime;
        public float Speed;
        public float RotationSpeed;
        public float ColliderRadius;
        [ReadOnly] 
        [NativeDisableContainerSafetyRestriction] 
        public ComponentLookup<LocalTransform> TransformLookup;
        [ReadOnly] public ComponentLookup<Character> Characters;
        [ReadOnly] public DynamicBuffer<CharacterConfig> CharacterConfigs;

        [BurstCompile]
        private void Execute(Entity entity, ref PhysicsVelocity physicsVelocity, in Target target, in DynamicBuffer<Ability> abilities, in Character character)
        {
            var targetCharacter = target.Value;
            var targetConfig = CharacterConfigs.Get(Characters[targetCharacter].ID);
            var characterConfig = CharacterConfigs.Get(character.ID);
            var baseDistance = abilities[0].Config.Radius + characterConfig.ColliderRadius + targetConfig.ColliderRadius;
            var stopDistance = abilities.Length > 0 && abilities[0].Config.Radius != 0 ? baseDistance : float.MaxValue;
            
            if (!TransformLookup.HasComponent(targetCharacter)) 
            {
                physicsVelocity.Linear = float3.zero;
                return;
            }

            LocalTransform targetTransform = TransformLookup[targetCharacter];
            float3 direction = targetTransform.Position - TransformLookup.GetRefRO(entity).ValueRO.Position;
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

            float3 targetDir = math.normalize(new float3(direction.x, 0f, direction.z));
            quaternion targetRot = quaternion.LookRotationSafe(targetDir, math.up());
            TransformLookup.GetRefRW(entity).ValueRW.Rotation = math.slerp(TransformLookup.GetRefRO(entity).ValueRO.Rotation, targetRot, RotationSpeed * DeltaTime);
            physicsVelocity.Angular = float3.zero;
        }
    }
}