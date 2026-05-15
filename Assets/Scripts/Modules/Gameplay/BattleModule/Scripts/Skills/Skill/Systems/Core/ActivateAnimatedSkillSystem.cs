using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateAfter(typeof(SkillCooldownSystem))]
    public partial struct ActivateAnimatedSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (events, skills, character) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>, DynamicBuffer<Skill>>().WithEntityAccess())
            {
                if (!HasAttackEvent(events)) continue;

                for (int i = 0; i < skills.Length; i++)
                {
                    ref var skill = ref skills.ElementAt(i);
                    if (!skill.IsPending) continue;
                    if (!SkillHandler.HasActivationAnimation(skill.Config.Value)) continue;

                    ActivateSkill(ecb, character, ref skill);
                }
            }
            ecb.Playback(state.EntityManager);
        }

        private static void ActivateSkill(EntityCommandBuffer ecb, Entity character, ref Skill skill)
        {
            var trigger = skill.Trigger;
            SkillHandler.ClearPending(ref skill);

            ecb.CreateFrameEntity(new SkillActivatedEvent
            {
                Character = character,
                Trigger = trigger,
                Skill = skill.Config
            });
        }

        private static bool HasAttackEvent(DynamicBuffer<AnimationEventComponent> events)
        {
            foreach (var evnt in events)
            {
                if (evnt.nameHash == "Attack".CalculateHash32()) return true;
            }
            return false;
        }
    }
}
