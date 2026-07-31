using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup), OrderFirst = true)]
    public partial struct SkillCooldownSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var (skills, statMultipliers, character, entity) in SystemAPI.Query<DynamicBuffer<Skill>, DynamicBuffer<StatMultiply>, RefRO<Character>>().WithEntityAccess())
            {
                var characterConfig = character.ValueRO.GetConfig();
                var activeSkillId = characterConfig.GetSkill(SkillSlotType.Active);
                var isDead = SystemAPI.HasComponent<Dead>(entity);

                for (int i = 0; i < skills.Length; i++)
                {
                    ref var skill = ref skills.ElementAt(i);

                    if (isDead) continue;

                    var skillConfig = skill.GetConfig();
                    if (skillConfig.Trigger == TriggerType.BattleStart) continue;
        
                    skill.Cooldown = math.min(skillConfig.Cooldown, skill.Cooldown + dt * SkillHandler.GetCooldownRate(activeSkillId, skillConfig.ID, statMultipliers));

                    if (skillConfig.Trigger != TriggerType.Cooldown) continue;
                    if (skillConfig.ID == activeSkillId) continue;
                    if (skill.Cooldown < skillConfig.Cooldown) continue;

                    ecb.CreateFrameEntity(new SkillCooldownEvent
                    {
                        Character = entity,
                        SkillID = skillConfig.ID
                    });
                }
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
