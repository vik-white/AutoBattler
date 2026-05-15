using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct MeleeAttackSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var targets = SystemAPI.GetComponentLookup<Target>(true);
            foreach (var skillActivatedEvent in SystemAPI.Query<RefRO<SkillActivatedEvent>>()) {
                var skill = skillActivatedEvent.ValueRO.Skill;
                var config = skill.Value;
                var entity = skillActivatedEvent.ValueRO.Character;
                if (config.Type != SkillType.MeleeAttack) continue;

                if (!SkillHandler.TryGetTarget(skill, entity, skillActivatedEvent.ValueRO.Trigger, targets, out var target)) continue;

                foreach (var status in config.Statuses) {
                    ecb.CreateFrameEntity(new CreateStatus
                    {
                        Skill = skill,
                        Provider = entity,
                        Target = target,
                        Data = status,
                    });
                }

                foreach (var effect in config.Effects) {
                    ecb.CreateFrameEntity(new CreateEffect
                    {
                        Skill = skill,
                        Provider = entity,
                        Target = target,
                        Data = effect,
                    });
                }

                foreach (var stat in config.Stats) {
                    ecb.CreateFrameEntity(new CreateStatChange
                    {
                        Skill = skill,
                        Provider = entity,
                        Target = target,
                        Data = stat,
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
