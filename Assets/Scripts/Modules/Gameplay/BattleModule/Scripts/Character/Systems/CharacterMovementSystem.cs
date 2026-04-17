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
        [ReadOnly] 
        [NativeDisableContainerSafetyRestriction] 
        public ComponentLookup<LocalTransform> TransformLookup;
        [ReadOnly] public ComponentLookup<Character> Characters;
        [ReadOnly] public DynamicBuffer<CharacterConfig> CharacterConfigs;

        [BurstCompile]
        private void Execute(Entity entity, ref PhysicsVelocity physicsVelocity, in ExternalVelocity externalVelocity, in Target target, in DynamicBuffer<Ability> abilities, in Character character)
        {
            var velocity = float3.zero;
            var targetCharacter = target.Value;
            var targetConfig = CharacterConfigs.Get(Characters[targetCharacter].ID);
            var characterConfig = CharacterConfigs.Get(character.ID);
            var abilityRadius = abilities.Length > 0 ? abilities[0].Config.Radius : 0;
            var baseDistance = abilityRadius + characterConfig.ColliderRadius + targetConfig.ColliderRadius;
            var stopDistance = abilities.Length > 0 && abilityRadius != 0 ? baseDistance : float.MaxValue;
            var targetTransform = TransformLookup[targetCharacter];
            var direction = targetTransform.Position - TransformLookup.GetRefRO(entity).ValueRO.Position;
            
            if (TransformLookup.HasComponent(targetCharacter)) 
            {
                float distance = math.length(direction);
                if (distance > stopDistance)
                {
                    float3 moveDir = math.normalize(new float3(direction.x, 0f, direction.z));
                    velocity = moveDir * Speed;
                }
            }
            float3 currentVelocity = physicsVelocity.Linear;
            physicsVelocity.Linear = new float3(
                velocity.x + externalVelocity.Value.x,
                currentVelocity.y + externalVelocity.Value.y,
                velocity.z + externalVelocity.Value.z);

            var targetDir = math.normalize(new float3(direction.x, 0f, direction.z));
            var targetRot = quaternion.LookRotationSafe(targetDir, math.up());
            TransformLookup.GetRefRW(entity).ValueRW.Rotation = math.slerp(TransformLookup.GetRefRO(entity).ValueRO.Rotation, targetRot, RotationSpeed * DeltaTime);
            physicsVelocity.Angular = float3.zero;
        }
    }
}