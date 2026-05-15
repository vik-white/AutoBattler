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
                SystemAPI.GetComponentLookup<Target>(true));

            var requests = new NativeList<SkillTriggerRequest>(Allocator.Temp);

            foreach (var cooldownEvent in SystemAPI.Query<RefRO<SkillCooldownEvent>>())
            {
                var source = cooldownEvent.ValueRO.Character;
                if (!context.IsAliveCharacter(source)) continue;
                requests.Add(new SkillTriggerRequest(source, TriggerType.Cooldown, cooldownEvent.ValueRO.SkillID));
            }

            foreach (var (activateEvent, eventEntity) in SystemAPI.Query<RefRO<ActivateSkillEvent>>().WithEntityAccess())
            {
                var source = activateEvent.ValueRO.Character;
                if (context.IsAliveCharacter(source)) 
                    requests.Add(new SkillTriggerRequest(source, TriggerType.Activate, activateEvent.ValueRO.SkillID, true));
                ecb.DestroyEntity(eventEntity);
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
                foreach (var (skills, statMultipliers, transform, character, owner) in SystemAPI.Query<DynamicBuffer<Skill>, DynamicBuffer<StatMultiply>, RefRO<LocalTransform>, RefRO<Character>>().WithEntityAccess())
                {
                    if (!SkillHandler.CanProcessOwner(owner, request, context)) continue;
                    TryTriggerSkills(ecb, skills, statMultipliers, owner, transform.ValueRO, character.ValueRO.GetConfig(), request, context);
                }
            }

            requests.Dispose();
            ecb.Playback(state.EntityManager);
        }

        private static void TryTriggerSkills(EntityCommandBuffer ecb, DynamicBuffer<Skill> skills, DynamicBuffer<StatMultiply> statMultipliers, Entity owner, in LocalTransform ownerTransform, in CharacterConfigData ownerConfig, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            var activeSkillId = ownerConfig.GetSkill(SkillSlotType.Active);

            for (int i = 0; i < skills.Length; i++)
            {
                ref var skill = ref skills.ElementAt(i);
                var skillConfig = skill.GetConfig();

                if (!SkillHandler.CanTriggerSkill(skill, skills, owner, ownerTransform, ownerConfig, skillConfig, request, context)) continue;

                skill.Cooldown = 0f;
                
                if (Random.value > skillConfig.Chance) continue;

                var speed = SkillHandler.GetCooldownRate(activeSkillId, skillConfig.ID, statMultipliers);
                StartSkill(ecb, skills, owner, request.TriggerEntity, ownerTransform.Position, ref skill, skillConfig, speed);
            }
        }

        private static void StartSkill(EntityCommandBuffer ecb, DynamicBuffer<Skill> skills, Entity entity, Entity trigger, float3 position, ref Skill skill, in SkillConfig skillConfig, float speed)
        {
            if (skillConfig.Type == SkillType.Skills)
            {
                StartSkills(ecb, skills, entity, trigger, position, speed);
                return;
            }

            StartSkill(ecb, entity, trigger, position, ref skill, speed);
        }

        private static void StartSkills(EntityCommandBuffer ecb, DynamicBuffer<Skill> skills, Entity entity, Entity trigger, float3 position, float speed)
        {
            for (int i = 0; i < skills.Length; i++)
            {
                ref var childSkill = ref skills.ElementAt(i);
                if (!childSkill.IsChild || childSkill.IsPending) continue;

                StartSkill(ecb, entity, trigger, position, ref childSkill, speed);
            }
        }

        private static void StartSkill(EntityCommandBuffer ecb, Entity entity, Entity trigger, float3 position, ref Skill skill, float speed)
        {
            skill.IsPending = true;
            skill.PendingTrigger = trigger;
            ecb.CreateFrameEntity(new SkillStartedEvent { Character = entity, Skill = skill.Config, Position = position, Speed = speed });
        }
    }
}
