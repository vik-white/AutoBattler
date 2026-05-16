using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SkillTriggerSystemGroup))]
    public partial struct SkillTriggerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var animatedSkills = new NativeList<StartedSkill>(Allocator.Temp);
            var startedSkills = new NativeList<StartedSkill>(Allocator.Temp);
            var startedAnimatedSkills = new NativeList<StartedSkill>(Allocator.Temp);
            var context = new SkillTriggerContext(
                SystemAPI.GetComponentLookup<LocalTransform>(true),
                SystemAPI.GetComponentLookup<Character>(true),
                SystemAPI.GetComponentLookup<Enemy>(true),
                SystemAPI.GetComponentLookup<Dead>(true),
                SystemAPI.GetComponentLookup<Target>(true));

            foreach (var (animatedSkill, character) in SystemAPI.Query<RefRO<SkillAnimated>>().WithEntityAccess())
                animatedSkills.Add(new StartedSkill { Character = character, SkillID = animatedSkill.ValueRO.Skill.Value.ID });

            var requests = new NativeList<SkillTriggerRequest>(Allocator.Temp);

            foreach (var cooldownEvent in SystemAPI.Query<RefRO<SkillCooldownEvent>>())
            {
                var source = cooldownEvent.ValueRO.Character;
                if (!context.IsAliveCharacter(source)) continue;
                requests.Add(new SkillTriggerRequest(source, TriggerType.Cooldown, cooldownEvent.ValueRO.SkillID));
            }

            foreach (var activateEvent in SystemAPI.Query<RefRO<ActivateSkillEvent>>())
            {
                var source = activateEvent.ValueRO.Character;
                if (!context.IsAliveCharacter(source)) continue;
                requests.Add(new SkillTriggerRequest(source, TriggerType.Activate, activateEvent.ValueRO.SkillID, true));
            }

            foreach (var damageEvent in SystemAPI.Query<RefRO<GetDamageEvent>>())
            {
                var source = damageEvent.ValueRO.Character;
                if (!context.Characters.HasComponent(source)) continue; 
                requests.Add(new SkillTriggerRequest(source, TriggerType.GetDamage, triggerEntity: damageEvent.ValueRO.Provider));
            }

            foreach (var deadEvent in SystemAPI.Query<RefRO<DeadCharacterEvent>>())
            {
                var source = deadEvent.ValueRO.Character;
                if (!context.Characters.HasComponent(source)) continue; 
                requests.Add(new SkillTriggerRequest(source, TriggerType.Dead, allowDeadSourceOwner: true));
            }

            foreach (var request in requests)
            {
                foreach (var (skills, instantSkills, statMultipliers, transform, character, owner) in SystemAPI.Query<DynamicBuffer<Skill>, DynamicBuffer<SkillInstant>, DynamicBuffer<StatMultiply>, RefRO<LocalTransform>, RefRO<Character>>().WithEntityAccess())
                {
                    if (!SkillHandler.CanProcessOwner(owner, request, context)) continue;
                    TryTriggerSkills(ecb, animatedSkills, startedSkills, startedAnimatedSkills, skills, instantSkills, statMultipliers, owner, transform.ValueRO, character.ValueRO.GetConfig(), request, context);
                }
            }

            requests.Dispose();
            startedAnimatedSkills.Dispose();
            startedSkills.Dispose();
            animatedSkills.Dispose();
            ecb.Playback(state.EntityManager);
        }

        private static void TryTriggerSkills(EntityCommandBuffer ecb, NativeList<StartedSkill> animatedSkills, NativeList<StartedSkill> startedSkills, NativeList<StartedSkill> startedAnimatedSkills, DynamicBuffer<Skill> skills, DynamicBuffer<SkillInstant> instantSkills, DynamicBuffer<StatMultiply> statMultipliers, Entity owner, in LocalTransform ownerTransform, in CharacterConfigData ownerConfig, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            var activeSkillId = ownerConfig.GetSkill(SkillSlotType.Active);

            for (int i = 0; i < skills.Length; i++)
            {
                ref var skill = ref skills.ElementAt(i);
                var skillConfig = skill.GetConfig();

                if (HasStartedSkill(startedSkills, owner, skillConfig)) continue;
                if (SkillHandler.HasActivationAnimation(skillConfig) && HasUnavailableAnimatedSkill(animatedSkills, startedAnimatedSkills, owner)) continue;
                if (!SkillHandler.CanTriggerSkill(skill, owner, ownerTransform, ownerConfig, skillConfig, request, context)) continue;

                skill.Cooldown = 0f;
                
                if (Random.value > skillConfig.Chance) continue;

                var speed = SkillHandler.GetCooldownRate(activeSkillId, skillConfig.ID, statMultipliers);
                StartSkill(ecb, startedSkills, startedAnimatedSkills, instantSkills, owner, request.TriggerEntity, ownerTransform.Position, skill, speed);
            }
        }

        private static void StartSkill(EntityCommandBuffer ecb, NativeList<StartedSkill> startedSkills, NativeList<StartedSkill> startedAnimatedSkills, DynamicBuffer<SkillInstant> instantSkills, Entity entity, Entity trigger, float3 position, in Skill skill, float speed)
        {
            startedSkills.Add(new StartedSkill { Character = entity, SkillID = skill.Config.Value.ID });
            ecb.CreateFrameEntity(new SkillStartedEvent { Character = entity, Skill = skill.Config, Position = position, Speed = speed });

            if (SkillHandler.HasActivationAnimation(skill.Config.Value))
            {
                var animatedSkill = new SkillAnimated
                {
                    Trigger = trigger,
                    Skill = skill.Config
                };

                startedAnimatedSkills.Add(new StartedSkill { Character = entity, SkillID = skill.Config.Value.ID });
                ecb.AddComponent(entity, animatedSkill);
                return;
            }

            instantSkills.Add(new SkillInstant
            {
                Trigger = trigger,
                Skill = skill.Config
            });
        }

        private static bool HasUnavailableAnimatedSkill(NativeList<StartedSkill> animatedSkills, NativeList<StartedSkill> startedAnimatedSkills, Entity character)
        {
            return HasAnimatedSkill(animatedSkills, character) || HasAnimatedSkill(startedAnimatedSkills, character);
        }

        private static bool HasAnimatedSkill(NativeList<StartedSkill> skills, Entity character)
        {
            foreach (var skill in skills)
            {
                if (skill.Character == character)
                    return true;
            }

            return false;
        }

        private static bool HasStartedSkill(NativeList<StartedSkill> startedSkills, Entity character, in SkillConfig skillConfig)
        {
            foreach (var startedSkill in startedSkills)
            {
                if (startedSkill.Character == character && startedSkill.SkillID == skillConfig.ID)
                    return true;
            }

            return false;
        }

        private struct StartedSkill
        {
            public Entity Character;
            public uint SkillID;
        }
    }
}
