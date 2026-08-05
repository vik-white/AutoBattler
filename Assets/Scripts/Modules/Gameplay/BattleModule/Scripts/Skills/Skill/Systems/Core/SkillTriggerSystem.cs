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
            var context = new SkillTriggerContext(
                SystemAPI.GetComponentLookup<LocalTransform>(true),
                SystemAPI.GetComponentLookup<Character>(true),
                SystemAPI.GetComponentLookup<Enemy>(true),
                SystemAPI.GetComponentLookup<Dead>(true),
                SystemAPI.GetComponentLookup<MovementLock>(true),
                SystemAPI.GetComponentLookup<ActiveSkillAnimationLock>(true),
                SystemAPI.GetComponentLookup<Target>(true));

            var requests = new NativeList<SkillTriggerRequest>(Allocator.Temp);

            foreach (var activateEvent in SystemAPI.Query<RefRO<ActivateSkillEvent>>())
            {
                var source = activateEvent.ValueRO.Character;
                if (!context.IsAliveCharacter(source)) continue;
                requests.Add(new SkillTriggerRequest(source, TriggerType.Activate, activateEvent.ValueRO.SkillID, true));
            }

            foreach (var cooldownEvent in SystemAPI.Query<RefRO<SkillCooldownEvent>>())
            {
                var source = cooldownEvent.ValueRO.Character;
                if (!context.IsAliveCharacter(source)) continue;
                requests.Add(new SkillTriggerRequest(source, TriggerType.Cooldown, cooldownEvent.ValueRO.SkillID));
            }

            foreach (var damageEvent in SystemAPI.Query<RefRO<GetDamageEvent>>())
            {
                var damagedCharacter = damageEvent.ValueRO.Character;
                if (!context.Characters.HasComponent(damagedCharacter)) continue;

                requests.Add(new SkillTriggerRequest(damagedCharacter, TriggerType.GetDamage, triggerEntity: damageEvent.ValueRO.Provider));

                if (damageEvent.ValueRO.HealthDroppedBelowHalf)
                    requests.Add(new SkillTriggerRequest(damagedCharacter, TriggerType.HPLessHalf, triggerEntity: damageEvent.ValueRO.Provider));

                if (!damageEvent.ValueRO.IsCrit) continue;

                var critProvider = damageEvent.ValueRO.Provider;
                if (!context.Characters.HasComponent(critProvider)) continue;

                requests.Add(new SkillTriggerRequest(critProvider, TriggerType.OnCrit, triggerEntity: damagedCharacter));
            }

            foreach (var deadEvent in SystemAPI.Query<RefRO<DeadCharacterEvent>>())
            {
                var source = deadEvent.ValueRO.Character;
                if (!context.Characters.HasComponent(source)) continue; 
                requests.Add(new SkillTriggerRequest(source, TriggerType.Dead, allowDeadSourceOwner: true));
            }

            var battleStartRequested = false;
            foreach (var (_, eventEntity) in SystemAPI.Query<RefRO<BattleStartEvent>>().WithEntityAccess())
            {
                battleStartRequested = true;
                ecb.DestroyEntity(eventEntity);
            }

            if (battleStartRequested)
            {
                foreach (var (skills, starterSkills, statMultipliers, transform, character, owner) in SystemAPI.Query<DynamicBuffer<Skill>, DynamicBuffer<StarterSkill>, DynamicBuffer<StatMultiply>, RefRO<LocalTransform>, RefRO<Character>>().WithNone<Stunned>().WithEntityAccess())
                {
                    var request = new SkillTriggerRequest(owner, TriggerType.BattleStart);
                    TriggerReadySkills(ecb, owner, transform.ValueRO, character.ValueRO.GetConfig(), skills, starterSkills, statMultipliers, request, context);
                }
            }

            foreach (var request in requests)
            {
                foreach (var (skills, starterSkills, statMultipliers, transform, character, owner) in SystemAPI.Query<DynamicBuffer<Skill>, DynamicBuffer<StarterSkill>, DynamicBuffer<StatMultiply>, RefRO<LocalTransform>, RefRO<Character>>().WithNone<Stunned>().WithEntityAccess())
                {
                    TriggerReadySkills(ecb, owner, transform.ValueRO, character.ValueRO.GetConfig(), skills, starterSkills, statMultipliers, request, context);
                }
            }

            requests.Dispose();
            ecb.Playback(state.EntityManager);
        }

        private static void TriggerReadySkills(EntityCommandBuffer ecb, Entity owner, in LocalTransform ownerTransform, in CharacterConfigData ownerConfig, DynamicBuffer<Skill> skills, DynamicBuffer<StarterSkill> starterSkills, DynamicBuffer<StatMultiply> statMultipliers, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            var activeSkillId = ownerConfig.GetSkill(SkillSlotType.Active);
            for (int i = 0; i < skills.Length; i++)
            {
                ref var skill = ref skills.ElementAt(i);
                var skillConfig = skill.GetConfig();

                if (!SkillHandler.CanTriggerSkill(skill, starterSkills, owner, ownerTransform, ownerConfig, skillConfig, request, context)) continue;

                if (request.Trigger == TriggerType.BattleStart)
                    skill.BattleStartTriggered = true;
                else
                    skill.Cooldown = 0f;
                
                if (Random.value > skillConfig.Chance) continue;

                skill.ActivatedCount++;
                var isManualActivation = request.Trigger == TriggerType.Activate && request.GetRequestedSkillID(owner) == skillConfig.ID;
                StartSkill(ecb, owner, request.Source, request.TriggerEntity, ownerTransform.Position, SkillHandler.GetCooldownRate(activeSkillId, skillConfig.ID, statMultipliers), starterSkills, skill.Config, isManualActivation);
            }
        }

        private static void StartSkill(EntityCommandBuffer ecb, Entity character, Entity triggerSource, Entity trigger, float3 position, float speed, DynamicBuffer<StarterSkill> starterSkills, BlobAssetReference<SkillConfig> skillConfig, bool isManualActivation)
        {
            if (isManualActivation)
                RemoveInterruptiblePendingAnimations(starterSkills);

            ecb.CreateFrameEntity(new StartedSkillEvent
            {
                Character = character,
                Skill = skillConfig,
                Position = position,
                Speed = speed
            });

            starterSkills.Add(new StarterSkill
            {
                TriggerSource = triggerSource,
                Trigger = trigger,
                Skill = skillConfig,
                WaitForAnimation = SkillHandler.HasActivationAnimation(skillConfig.Value)
            });
        }

        private static void RemoveInterruptiblePendingAnimations(DynamicBuffer<StarterSkill> starterSkills)
        {
            for (int i = 0; i < starterSkills.Length;)
            {
                var starterSkill = starterSkills[i];
                if (starterSkill.WaitForAnimation && starterSkill.Skill.Value.Trigger != TriggerType.Activate)
                {
                    starterSkills.RemoveAt(i);
                    continue;
                }

                i++;
            }
        }
    }
}
