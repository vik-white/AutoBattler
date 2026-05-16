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
                foreach (var (skills, pendingSkills, statMultipliers, transform, character, owner) in SystemAPI.Query<DynamicBuffer<Skill>, DynamicBuffer<PendingSkill>, DynamicBuffer<StatMultiply>, RefRO<LocalTransform>, RefRO<Character>>().WithEntityAccess())
                {
                    var skillOwner = new SkillOwner
                    {
                        Entity = owner,
                        Transform = transform.ValueRO,
                        Config = character.ValueRO.GetConfig(),
                        Skills = skills,
                        PendingSkills = pendingSkills,
                        StatMultipliers = statMultipliers,
                    };

                    TriggerReadySkills(ecb, skillOwner, request, context);
                }
            }

            requests.Dispose();
            ecb.Playback(state.EntityManager);
        }

        private static void TriggerReadySkills(EntityCommandBuffer ecb, SkillOwner owner, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            if (!SkillHandler.CanProcessOwner(owner.Entity, request, context)) return;

            var activeSkillId = owner.Config.GetSkill(SkillSlotType.Active);

            for (int i = 0; i < owner.Skills.Length; i++)
            {
                ref var skill = ref owner.Skills.ElementAt(i);
                var skillConfig = skill.GetConfig();

                if (!SkillHandler.CanTriggerSkill(skill, owner.PendingSkills, owner.Entity, owner.Transform, owner.Config, skillConfig, request, context)) continue;

                skill.Cooldown = 0f;
                
                if (Random.value > skillConfig.Chance) continue;

                StartSkill(ecb, owner.PendingSkills, new SkillStart
                {
                    Character = owner.Entity,
                    Trigger = request.TriggerEntity,
                    Position = owner.Transform.Position,
                    Skill = skill.Config,
                    Speed = SkillHandler.GetCooldownRate(activeSkillId, skillConfig.ID, owner.StatMultipliers)
                });
            }
        }

        private static void StartSkill(EntityCommandBuffer ecb, DynamicBuffer<PendingSkill> pendingSkills, in SkillStart start)
        {
            ecb.CreateFrameEntity(new SkillStartedEvent
            {
                Character = start.Character,
                Skill = start.Skill,
                Position = start.Position,
                Speed = start.Speed
            });

            pendingSkills.Add(new PendingSkill
            {
                Trigger = start.Trigger,
                Skill = start.Skill,
                WaitForAnimation = SkillHandler.HasActivationAnimation(start.Skill.Value)
            });
        }

        private struct SkillOwner
        {
            public Entity Entity;
            public LocalTransform Transform;
            public CharacterConfigData Config;
            public DynamicBuffer<Skill> Skills;
            public DynamicBuffer<PendingSkill> PendingSkills;
            public DynamicBuffer<StatMultiply> StatMultipliers;
        }

        private struct SkillStart
        {
            public Entity Character;
            public Entity Trigger;
            public float3 Position;
            public BlobAssetReference<SkillConfig> Skill;
            public float Speed;
        }
    }
}
