using System.Collections.Generic;
using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(AnimationSystemGroup))]
    [UpdateBefore(typeof(CharacterAnimationSystem))]
    public partial struct CharacterMovementLockSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var movementLocks = SystemAPI.GetComponentLookup<MovementLock>(true);
            var startedLocks = new HashSet<Entity>();
            var endedLocks = new HashSet<Entity>();
            var endHash = "End".CalculateHash32();

            foreach (var (events, entity) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>>().WithEntityAccess())
            {
                if (!movementLocks.HasComponent(entity)) continue;

                foreach (var evnt in events)
                {
                    if (evnt.nameHash != endHash) continue;
                    endedLocks.Add(entity);
                    break;
                }
            }

            foreach (var skillStartedEvent in SystemAPI.Query<RefRO<StartedSkillEvent>>())
            {
                var skillConfig = skillStartedEvent.ValueRO.Skill.Value;
                if (!ShouldLockMovement(skillConfig.Animation)) continue;
                startedLocks.Add(skillStartedEvent.ValueRO.Character);
            }

            foreach (var entity in endedLocks)
            {
                if (!startedLocks.Contains(entity))
                    ecb.RemoveComponent<MovementLock>(entity);
            }

            foreach (var entity in startedLocks)
            {
                if (!movementLocks.HasComponent(entity))
                    ecb.AddComponent<MovementLock>(entity);
            }

            ecb.Playback(state.EntityManager);
        }

        private static bool ShouldLockMovement(AnimationType animation)
        {
            return animation == AnimationType.Attack || animation == AnimationType.Ability;
        }
    }
}
