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

            foreach (var (pendingSkills, character) in SystemAPI.Query<DynamicBuffer<StarterSkill>>().WithNone<Stunned>().WithEntityAccess())
                ActivateReadySkills(ecb, character, pendingSkills, skillRuntimeData, false);

            foreach (var (events, pendingSkills, character) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>, DynamicBuffer<StarterSkill>>().WithNone<Stunned>().WithEntityAccess())
            {
                if (!HasAttackEvent(events)) continue;
                ActivateReadySkills(ecb, character, pendingSkills, skillRuntimeData, true);
            }
            ecb.Playback(state.EntityManager);
        }

        private static void ActivateReadySkills(EntityCommandBuffer ecb, Entity character, DynamicBuffer<StarterSkill> pendingSkills, DynamicBuffer<SkillRuntimeData> skillRuntimeData, bool waitForAnimation)
        {
            for (int i = 0; i < pendingSkills.Length;)
            {
                var pendingSkill = pendingSkills[i];
                if (pendingSkill.WaitForAnimation != waitForAnimation)
                {
                    i++;
                    continue;
                }

                ActivateSkill(ecb, character, pendingSkill.TriggerSource, pendingSkill.Trigger, pendingSkill.Skill, skillRuntimeData);
                pendingSkills.RemoveAt(i);
            }
        }

        private static void ActivateSkill(EntityCommandBuffer ecb, Entity character, Entity triggerSource, Entity trigger, BlobAssetReference<SkillConfig> skill, DynamicBuffer<SkillRuntimeData> skillRuntimeData)
        {
            var skillConfig = skill.Value;
            if (skillConfig.Skills.Length == 0)
            {
                CreateSkillActivatedEvent(ecb, character, triggerSource, trigger, skill);
                return;
            }

            for (int i = 0; i < skillConfig.Skills.Length; i++)
            {
                var nestedSkill = skillRuntimeData.Get(skillConfig.Skills[i]);
                var delay = i < skillConfig.SkillDelays.Length ? skillConfig.SkillDelays[i] : 0f;
                if (delay <= 0f)
                {
                    CreateSkillActivatedEvent(ecb, character, triggerSource, trigger, nestedSkill);
                    continue;
                }

                ecb.CreateSceneEntity(new DelayedSkillActivation
                {
                    Character = character,
                    TriggerSource = triggerSource,
                    Trigger = trigger,
                    Skill = nestedSkill,
                    TimeLeft = delay
                });
            }
        }

        private static void CreateSkillActivatedEvent(EntityCommandBuffer ecb, Entity character, Entity triggerSource, Entity trigger, BlobAssetReference<SkillConfig> skill)
        {
            ecb.CreateFrameEntity(new SkillActivatedEvent
            {
                Character = character,
                TriggerSource = triggerSource,
                Trigger = trigger,
                Skill = skill
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

    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateBefore(typeof(ActivateSkillSystem))]
    public partial struct DelayedSkillActivationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (delayedSkill, entity) in SystemAPI.Query<RefRW<DelayedSkillActivation>>().WithEntityAccess())
            {
                delayedSkill.ValueRW.TimeLeft -= deltaTime;
                if (delayedSkill.ValueRO.TimeLeft > 0f) continue;

                ecb.CreateFrameEntity(new SkillActivatedEvent
                {
                    Character = delayedSkill.ValueRO.Character,
                    TriggerSource = delayedSkill.ValueRO.TriggerSource,
                    Trigger = delayedSkill.ValueRO.Trigger,
                    Skill = delayedSkill.ValueRO.Skill
                });
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
