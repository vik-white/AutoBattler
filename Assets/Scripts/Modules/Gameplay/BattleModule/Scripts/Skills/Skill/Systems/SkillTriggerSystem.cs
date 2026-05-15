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
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);
            var enemies = SystemAPI.GetComponentLookup<Enemy>(true);
            var dead = SystemAPI.GetComponentLookup<Dead>(true);
            var targets = SystemAPI.GetComponentLookup<Target>(true);

            foreach (var cooldownEvent in SystemAPI.Query<RefRO<SkillCooldownEvent>>())
            {
                var source = cooldownEvent.ValueRO.Character;
                if (!characters.HasComponent(source) || dead.HasComponent(source)) continue;

                foreach (var (skills, statMultipliers, transform, character, owner) in SystemAPI.Query<DynamicBuffer<Skill>, DynamicBuffer<StatMultiply>, RefRO<LocalTransform>, RefRO<Character>>().WithNone<Dead>().WithEntityAccess())
                {
                    TryTriggerSkills(ecb, skills, statMultipliers, owner, transform.ValueRO, character.ValueRO.GetConfig(), source, TriggerType.Cooldown, owner == source ? cooldownEvent.ValueRO.SkillID : 0, false, transforms, characters, enemies, dead, targets);
                }
            }

            foreach (var (activateEvent, eventEntity) in SystemAPI.Query<RefRO<ActivateSkillEvent>>().WithEntityAccess())
            {
                var source = activateEvent.ValueRO.Character;
                if (!characters.HasComponent(source) || dead.HasComponent(source))
                {
                    ecb.DestroyEntity(eventEntity);
                    continue;
                }

                foreach (var (skills, statMultipliers, transform, character, owner) in SystemAPI.Query<DynamicBuffer<Skill>, DynamicBuffer<StatMultiply>, RefRO<LocalTransform>, RefRO<Character>>().WithNone<Dead>().WithEntityAccess())
                {
                    TryTriggerSkills(ecb, skills, statMultipliers, owner, transform.ValueRO, character.ValueRO.GetConfig(), source, TriggerType.Activate, owner == source ? activateEvent.ValueRO.SkillID : 0, owner == source, transforms, characters, enemies, dead, targets);
                }

                ecb.DestroyEntity(eventEntity);
            }

            foreach (var damageEvent in SystemAPI.Query<RefRO<GetDamageEvent>>())
            {
                var source = damageEvent.ValueRO.Character;
                if (!characters.HasComponent(source)) continue;

                foreach (var (skills, statMultipliers, transform, character, owner) in SystemAPI.Query<DynamicBuffer<Skill>, DynamicBuffer<StatMultiply>, RefRO<LocalTransform>, RefRO<Character>>().WithNone<Dead>().WithEntityAccess())
                {
                    TryTriggerSkills(ecb, skills, statMultipliers, owner, transform.ValueRO, character.ValueRO.GetConfig(), source, TriggerType.GetDamage, 0, false, transforms, characters, enemies, dead, targets);
                }
            }

            foreach (var deadEvent in SystemAPI.Query<RefRO<DeadCharacterEvent>>())
            {
                var source = deadEvent.ValueRO.Character;
                if (!characters.HasComponent(source)) continue;

                foreach (var (skills, statMultipliers, transform, character, owner) in SystemAPI.Query<DynamicBuffer<Skill>, DynamicBuffer<StatMultiply>, RefRO<LocalTransform>, RefRO<Character>>().WithEntityAccess())
                {
                    if (dead.HasComponent(owner) && owner != source) continue;

                    TryTriggerSkills(ecb, skills, statMultipliers, owner, transform.ValueRO, character.ValueRO.GetConfig(), source, TriggerType.Dead, 0, false, transforms, characters, enemies, dead, targets);
                }
            }

            ecb.Playback(state.EntityManager);
        }

        private static void TryTriggerSkills(
            EntityCommandBuffer ecb,
            DynamicBuffer<Skill> skills,
            DynamicBuffer<StatMultiply> statMultipliers,
            Entity owner,
            in LocalTransform ownerTransform,
            in CharacterConfigData ownerConfig,
            Entity eventSource,
            TriggerType trigger,
            uint requestedSkillId,
            bool ignoreRadius,
            in ComponentLookup<LocalTransform> transforms,
            in ComponentLookup<Character> characters,
            in ComponentLookup<Enemy> enemies,
            in ComponentLookup<Dead> dead,
            in ComponentLookup<Target> targets)
        {
            var activeSkillId = ownerConfig.GetSkill(SkillSlotType.Active);

            for (int i = 0; i < skills.Length; i++)
            {
                ref var skill = ref skills.ElementAt(i);
                if (skill.IsChild) continue;

                var skillConfig = skill.GetConfig();
                if (requestedSkillId != 0 && skillConfig.ID != requestedSkillId) continue;
                if (skillConfig.Trigger != trigger) continue;
                if (!MatchesTriggerSource(owner, eventSource, skillConfig.TriggerSource, enemies, characters)) continue;
                if (skill.Cooldown < skillConfig.Cooldown) continue;
                if (!CanUseSkill(owner, ownerTransform, skillConfig, ownerConfig, ignoreRadius, transforms, characters, dead, targets)) continue;

                if (Random.value > skillConfig.Chance)
                {
                    skill.Cooldown = 0f;
                    continue;
                }

                skill.Cooldown = 0f;
                var speed = SkillCooldownSystem.GetCooldownRate(activeSkillId, skillConfig.ID, statMultipliers);
                TriggerSkill(ecb, skills, owner, ownerTransform.Position, skillConfig, speed, ref skill);
            }
        }

        private static bool MatchesTriggerSource(Entity owner, Entity source, TargetType triggerSource, in ComponentLookup<Enemy> enemies, in ComponentLookup<Character> characters)
        {
            if (source == Entity.Null || !characters.HasComponent(source)) return false;

            if (triggerSource == TargetType.Self) return source == owner;
            if (source == owner) return false;

            var ownerIsEnemy = enemies.HasComponent(owner);
            var sourceIsEnemy = enemies.HasComponent(source);

            return triggerSource switch
            {
                TargetType.Allies => ownerIsEnemy == sourceIsEnemy,
                TargetType.Enemies => ownerIsEnemy != sourceIsEnemy,
                _ => false
            };
        }

        private static bool CanUseSkill(
            Entity owner,
            in LocalTransform ownerTransform,
            in SkillConfig skillConfig,
            in CharacterConfigData ownerConfig,
            bool ignoreRadius,
            in ComponentLookup<LocalTransform> transforms,
            in ComponentLookup<Character> characters,
            in ComponentLookup<Dead> dead,
            in ComponentLookup<Target> targets)
        {
            if (!NeedsTarget(skillConfig)) return true;
            if (!targets.HasComponent(owner)) return false;

            var target = targets[owner].Value;
            if (target == Entity.Null) return false;
            if (dead.HasComponent(target)) return false;
            if (!transforms.HasComponent(target) || !characters.HasComponent(target)) return false;
            if (ignoreRadius || skillConfig.Radius == 0f) return true;

            var targetConfig = characters[target].GetConfig();
            var distance = math.distance(ownerTransform.Position, transforms[target].Position);
            var maxDistance = skillConfig.Radius + ownerConfig.ColliderRadius + targetConfig.ColliderRadius;
            return distance <= maxDistance;
        }

        private static bool NeedsTarget(in SkillConfig skillConfig)
        {
            if (skillConfig.Type is SkillType.MeleeAttack or SkillType.RangeAttack) return true;
            if (skillConfig.Type != SkillType.Skills) return false;

            foreach (var target in skillConfig.Targets)
                if (target == TargetType.Enemies)
                    return true;

            return false;
        }

        private static void TriggerSkill(EntityCommandBuffer ecb, DynamicBuffer<Skill> skills, Entity entity, float3 position, in SkillConfig skillConfig, float speed, ref Skill skill)
        {
            if (skillConfig.Type != SkillType.Skills)
            {
                skill.IsActivated = false;
                skill.IsAnimating = true;
                ecb.CreateFrameEntity(new SkillStartedEvent { Character = entity, Skill = skill.Config, Position = position, Speed = speed });
                return;
            }

            for (int i = 0; i < skills.Length; i++)
            {
                ref var childSkill = ref skills.ElementAt(i);
                if (!childSkill.IsChild) continue;

                childSkill.IsActivated = false;
                childSkill.IsAnimating = true;
                ecb.CreateFrameEntity(new SkillStartedEvent { Character = entity, Skill = childSkill.Config, Position = position, Speed = speed });
            }
        }
    }
}
