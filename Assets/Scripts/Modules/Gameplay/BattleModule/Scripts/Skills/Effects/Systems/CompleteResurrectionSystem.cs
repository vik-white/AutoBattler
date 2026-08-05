using Rukhanka;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(ResurrectionEffectSystem))]
    public partial struct CompleteResurrectionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var healths = SystemAPI.GetComponentLookup<Health>();
            var healthMaxes = SystemAPI.GetComponentLookup<HealthMax>(true);
            var colliders = SystemAPI.GetComponentLookup<PhysicsCollider>(true);
            var movementLocks = SystemAPI.GetComponentLookup<MovementLock>(true);
            var activeSkillAnimationLocks = SystemAPI.GetComponentLookup<ActiveSkillAnimationLock>(true);
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (dead, pendingResurrection, parameters, layers, entity) in
                     SystemAPI.Query<
                             RefRO<Dead>,
                             RefRO<PendingResurrection>,
                             DynamicBuffer<AnimatorControllerParameterComponent>,
                             DynamicBuffer<AnimatorControllerLayerComponent>>()
                         .WithEntityAccess())
            {
                if (!dead.ValueRO.AnimationCompleted
                    || !healths.HasComponent(entity)
                    || !healthMaxes.HasComponent(entity))
                    continue;

                var health = healthMaxes[entity].Value * math.saturate(pendingResurrection.ValueRO.HealthPercentage);
                if (health <= 0f) continue;

                ResetToIdle(parameters, layers);
                healths[entity] = new Health { Value = health };

                if (!colliders.HasComponent(entity) && dead.ValueRO.Collider.Value.IsCreated)
                    ecb.AddComponent(entity, dead.ValueRO.Collider);

                if (movementLocks.HasComponent(entity))
                    ecb.RemoveComponent<MovementLock>(entity);

                if (activeSkillAnimationLocks.HasComponent(entity))
                    ecb.RemoveComponent<ActiveSkillAnimationLock>(entity);

                if (state.EntityManager.HasComponent<Destroy>(entity))
                    ecb.RemoveComponent<Destroy>(entity);

                ecb.RemoveComponent<PendingResurrection>(entity);
                ecb.RemoveComponent<Dead>(entity);
                ecb.CreateFrameEntity(new ResurrectCharacterEvent { Character = entity });
            }

            ecb.Playback(state.EntityManager);
        }

        private static void ResetToIdle(
            DynamicBuffer<AnimatorControllerParameterComponent> parameters,
            DynamicBuffer<AnimatorControllerLayerComponent> layers)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                if (parameter.type is ControllerParameterType.Bool or ControllerParameterType.Trigger)
                    parameter.BoolValue = false;
                parameters[i] = parameter;
            }

            for (var i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                layer.rtd = RuntimeAnimatorData.MakeDefault();
                layer.speed = 1f;
                layers[i] = layer;
            }
        }
    }
}
