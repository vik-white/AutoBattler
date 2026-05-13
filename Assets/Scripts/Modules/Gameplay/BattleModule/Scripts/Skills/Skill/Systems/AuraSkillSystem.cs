using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct AuraSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (skills, entity) in SystemAPI.Query<DynamicBuffer<Skill>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var skill in skills) {
                    if (!skill.TryGetActivatedConfig(SkillType.Aura, out var config)) continue;
                    ecb.CreateFrameEntity(new CreateAura()
                    {
                        Provider = entity,
                        Skill = skill.Config,
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
