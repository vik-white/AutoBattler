using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct BuffSkillSystem : ISystem
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
                if (config.Type != SkillType.Buff) continue;
                if (config.Targets.Length == 0) continue;

                NativeArray<Entity> enemies = SystemAPI.QueryBuilder().WithAll<Character>().WithAny<Enemy>().Build().ToEntityArray(Allocator.Temp);
                NativeArray<Entity> allies = SystemAPI.QueryBuilder().WithAll<Character>().WithNone<Enemy>().Build().ToEntityArray(Allocator.Temp);
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

                foreach (var status in config.Statuses) {
                    foreach (var target in targets)
                    {
                        ecb.CreateFrameEntity(new CreateStatus
                        {
                            Skill = skill,
                            Provider = entity,
                            Target = target,
                            Data = status,
                        });
                    }
                }

                foreach (var effect in config.Effects) {
                    foreach (var target in targets)
                    {
                        ecb.CreateFrameEntity(new CreateEffect
                        {
                            Skill = skill,
                            Provider = entity,
                            Target = target,
                            Data = effect,
                        });
                    }
                }

                foreach (var stat in config.Stats) {
                    foreach (var target in targets)
                    {
                        ecb.CreateFrameEntity(new CreateStatChange
                        {
                            Skill = skill,
                            Provider = entity,
                            Target = target,
                            Data = stat,
                        });
                    }
                }

                enemies.Dispose();
                allies.Dispose();
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
