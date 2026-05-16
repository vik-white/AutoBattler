using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateAfter(typeof(SkillCooldownSystem))]
    public partial struct ActivateSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (instantSkills, character) in SystemAPI.Query<DynamicBuffer<SkillInstant>>().WithEntityAccess())
                ActivateInstantSkills(ecb, character, instantSkills);

            foreach (var (events, animatedSkill, character) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>, RefRO<SkillAnimated>>().WithEntityAccess())
            {
                if (!HasAttackEvent(events)) continue;
                ActivateSkill(ecb, character, animatedSkill.ValueRO);
            }
            ecb.Playback(state.EntityManager);
        }

        private static void ActivateInstantSkills(EntityCommandBuffer ecb, Entity character, DynamicBuffer<SkillInstant> skills)
        {
            foreach (var skill in skills)
                ActivateSkill(ecb, character, skill.Trigger, skill.Skill, skill.InheritedSkills);

            skills.Clear();
        }

        private static void ActivateSkill(EntityCommandBuffer ecb, Entity character, in SkillAnimated skill)
        {
            ActivateSkill(ecb, character, skill.Trigger, skill.Skill, skill.InheritedSkills);
            ecb.RemoveComponent<SkillAnimated>(character);
        }

        private static void ActivateSkill(EntityCommandBuffer ecb, Entity character, Entity trigger, BlobAssetReference<SkillConfig> skill, in FixedList128Bytes<BlobAssetReference<SkillConfig>> inheritedSkills)
        {
            ecb.CreateFrameEntity(new SkillActivatedEvent
            {
                Character = character,
                Trigger = trigger,
                Skill = skill
            });

            for (int i = 0; i < inheritedSkills.Length; i++)
            {
                ecb.CreateFrameEntity(new SkillActivatedEvent
                {
                    Character = character,
                    Trigger = trigger,
                    Skill = inheritedSkills[i]
                });
            }
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
