using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct AuraSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var skillActivatedEvent in SystemAPI.Query<RefRO<SkillActivatedEvent>>()) {
                var skill = skillActivatedEvent.ValueRO.Skill;
                if (skill.Value.Type != SkillType.Aura) continue;

                ecb.CreateFrameEntity(new CreateAura()
                {
                    Provider = skillActivatedEvent.ValueRO.Character,
                    Skill = skill,
                });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
