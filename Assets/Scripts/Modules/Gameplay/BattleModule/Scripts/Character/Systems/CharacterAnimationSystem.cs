using System.Collections.Generic;
using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(AnimationSystemGroup))]
    public partial struct CharacterAnimationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var movementLocks = SystemAPI.GetComponentLookup<MovementLock>(true);
            var movementLockRequests = new HashSet<Entity>();

            foreach (var skillStartedEvent in SystemAPI.Query<RefRO<SkillStartedEvent>>())
            {
                var skillConfig = skillStartedEvent.ValueRO.Skill.Value;
                var character = skillStartedEvent.ValueRO.Character;

                AddMovementLockIfNeeded(ecb, character, skillConfig.Animation, movementLocks, movementLockRequests);
                PlayAnimation(ref state, character, skillConfig.Animation, skillStartedEvent.ValueRO.Speed);
            }

            foreach (var deadEvent in SystemAPI.Query<RefRO<DeadCharacterEvent>>())
                PlayAnimation(ref state, deadEvent.ValueRO.Character, AnimationType.Dead, 1f);

            foreach (var visualEvent in SystemAPI.Query<RefRO<CreateEffectEvent>>())
                PlayAnimation(ref state, visualEvent.ValueRO.Target, AnimationType.Reaction, 1f);

            ecb.Playback(state.EntityManager);
        }

        private static void AddMovementLockIfNeeded(
            EntityCommandBuffer ecb,
            Entity character,
            AnimationType animation,
            in ComponentLookup<MovementLock> movementLocks,
            HashSet<Entity> movementLockRequests)
        {
            if (animation != AnimationType.Attack && animation != AnimationType.Ability) return;
            if (!movementLocks.HasComponent(character) && movementLockRequests.Add(character))
                ecb.AddComponent<MovementLock>(character);
        }

        private void PlayAnimation(ref SystemState state, Entity character, AnimationType animation, float speed)
        {
            var animator = SystemAPI.GetBuffer<AnimatorControllerParameterComponent>(character);
            var param = animator[(int)animation];
            param.SetTrigger();
            animator[(int)animation] = param;

            var animatorLayers = SystemAPI.GetBuffer<AnimatorControllerLayerComponent>(character);
            var layer = animatorLayers[0];
            layer.speed = speed;
            animatorLayers[0] = layer;
        }
    }
}
