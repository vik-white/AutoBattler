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
            foreach (var (skills, character) in SystemAPI.Query<DynamicBuffer<Skill>>().WithEntityAccess())
            {
                ActivateInstantSkills(ecb, skills, character);
            }

            foreach (var (events, skills, character) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>, DynamicBuffer<Skill>>().WithEntityAccess())
            {
                if (!HasAttackEvent(events)) continue;

                for (int i = 0; i < skills.Length; i++)
                {
                    ref var skill = ref skills.ElementAt(i);
                    if (!skill.IsPending) continue;
                    if (!skill.Config.IsCreated) continue;
                    if (!SkillHandler.HasActivationAnimation(skill.Config.Value)) continue;

                    ActivateSkill(ecb, character, ref skill);
                }
            }
            ecb.Playback(state.EntityManager);
        }

        private static void ActivateInstantSkills(EntityCommandBuffer ecb, DynamicBuffer<Skill> skills, Entity character)
        {
            for (int i = 0; i < skills.Length; i++)
            {
                ref var skill = ref skills.ElementAt(i);
                if (!skill.IsPending) continue;
                if (!skill.Config.IsCreated)
                {
                    ClearPending(ref skill);
                    continue;
                }

                if (SkillHandler.HasActivationAnimation(skill.Config.Value)) continue;

                ActivateSkill(ecb, character, ref skill);
            }
        }

        private static void ActivateSkill(EntityCommandBuffer ecb, Entity character, ref Skill skill)
        {
            var trigger = skill.PendingTrigger;
            ClearPending(ref skill);

            ecb.CreateFrameEntity(new SkillActivatedEvent
            {
                Character = character,
                Trigger = trigger,
                Skill = skill.Config
            });
        }

        private static void ClearPending(ref Skill skill)
        {
            skill.IsPending = false;
            skill.PendingTrigger = Entity.Null;
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
