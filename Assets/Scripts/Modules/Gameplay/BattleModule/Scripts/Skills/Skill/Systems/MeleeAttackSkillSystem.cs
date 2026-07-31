using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct MeleeAttackSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var selectedTargets = SystemAPI.GetComponentLookup<Target>(true);
            var healths = SystemAPI.GetComponentLookup<Health>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);
            var statMultipliers = SystemAPI.GetBufferLookup<StatMultiply>(true);
            foreach (var skillActivatedEvent in SystemAPI.Query<RefRO<SkillActivatedEvent>>()) {
                var skill = skillActivatedEvent.ValueRO.Skill;
                var config = skill.Value;
                var entity = skillActivatedEvent.ValueRO.Character;
                if (config.Type != SkillType.MeleeAttack) continue;

                if (config.TargetsCount <= 0)
                {
                    if (!SkillHandler.TryGetTarget(skill, entity, skillActivatedEvent.ValueRO.Trigger, selectedTargets, out var target))
                        continue;

                    ApplySkill(ecb, skill, entity, target);
                    continue;
                }

                using var enemies = SystemAPI.QueryBuilder().WithAll<Character>().WithAny<Enemy>().Build().ToEntityArray(Allocator.Temp);
                using var allies = SystemAPI.QueryBuilder().WithAll<Character>().WithNone<Enemy>().Build().ToEntityArray(Allocator.Temp);
                var targets = SkillHandler.GetTargets(
                    skill,
                    entity,
                    skillActivatedEvent.ValueRO.Trigger,
                    selectedTargets,
                    healths,
                    characters,
                    statMultipliers,
                    SystemAPI.HasComponent<Enemy>(entity),
                    enemies,
                    allies);

                foreach (var target in targets)
                    ApplySkill(ecb, skill, entity, target);
            }
            ecb.Playback(state.EntityManager);
        }

        private static void ApplySkill(EntityCommandBuffer ecb, BlobAssetReference<SkillConfig> skill, Entity provider, Entity target)
        {
            var config = skill.Value;
            foreach (var status in config.Statuses) {
                ecb.CreateFrameEntity(new CreateStatus
                {
                    Skill = skill,
                    Provider = provider,
                    Target = target,
                    Data = status,
                });
            }

            foreach (var effect in config.Effects) {
                ecb.CreateFrameEntity(new CreateEffect
                {
                    Skill = skill,
                    Provider = provider,
                    Target = target,
                    Data = effect,
                });
            }

            foreach (var stat in config.Stats) {
                ecb.CreateFrameEntity(new CreateStatChange
                {
                    Skill = skill,
                    Provider = provider,
                    Target = target,
                    Data = stat,
                });
            }
        }
    }
}
