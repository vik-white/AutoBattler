using Rukhanka;
using Rukhanka.Toolbox;
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
            var skillRuntimeData = SystemAPI.GetSingletonBuffer<SkillRuntimeData>(true);

            foreach (var (instantSkills, character) in SystemAPI.Query<DynamicBuffer<SkillInstant>>().WithEntityAccess())
                ActivateInstantSkills(ecb, character, instantSkills, skillRuntimeData);

            foreach (var (events, animatedSkill, character) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>, RefRO<SkillAnimated>>().WithEntityAccess())
            {
                if (!HasAttackEvent(events)) continue;
                ActivateSkill(ecb, character, animatedSkill.ValueRO, skillRuntimeData);
            }
            ecb.Playback(state.EntityManager);
        }

        private static void ActivateInstantSkills(EntityCommandBuffer ecb, Entity character, DynamicBuffer<SkillInstant> skills, DynamicBuffer<SkillRuntimeData> skillRuntimeData)
        {
            foreach (var skill in skills)
                ActivateSkill(ecb, character, skill.Trigger, skill.Skill, skillRuntimeData);

            skills.Clear();
        }

        private static void ActivateSkill(EntityCommandBuffer ecb, Entity character, in SkillAnimated skill, DynamicBuffer<SkillRuntimeData> skillRuntimeData)
        {
            ActivateSkill(ecb, character, skill.Trigger, skill.Skill, skillRuntimeData);
            ecb.RemoveComponent<SkillAnimated>(character);
        }

        private static void ActivateSkill(EntityCommandBuffer ecb, Entity character, Entity trigger, BlobAssetReference<SkillConfig> skill, DynamicBuffer<SkillRuntimeData> skillRuntimeData)
        {
            ecb.CreateFrameEntity(new SkillActivatedEvent
            {
                Character = character,
                Trigger = trigger,
                Skill = skill
            });

            var skillConfig = skill.Value;
            for (int i = 0; i < skillConfig.Skills.Length; i++)
            {
                ecb.CreateFrameEntity(new SkillActivatedEvent
                {
                    Character = character,
                    Trigger = trigger,
                    Skill = skillRuntimeData.Get(skillConfig.Skills[i])
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
