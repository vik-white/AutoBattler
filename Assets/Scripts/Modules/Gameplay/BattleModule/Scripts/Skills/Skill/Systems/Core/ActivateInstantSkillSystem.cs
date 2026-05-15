using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateAfter(typeof(ClearPendingSkillsSystem))]
    [UpdateBefore(typeof(ActivateAnimatedSkillSystem))]
    public partial struct ActivateInstantSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (skills, character) in SystemAPI.Query<DynamicBuffer<Skill>>().WithEntityAccess())
            {
                for (int i = 0; i < skills.Length; i++)
                {
                    ref var skill = ref skills.ElementAt(i);
                    if (!skill.IsPending) continue;

                    if (!skill.Config.IsCreated)
                    {
                        ClearPending(ref skill);
                        continue;
                    }

                    if (SkillHandler.HasActivationAnimation(skill.Config.Value)) continue;

                    ActivateSkill(ecb, character, ref skill);
                }
            }

            ecb.Playback(state.EntityManager);
        }

        private static void ActivateSkill(EntityCommandBuffer ecb, Entity character, ref Skill skill)
        {
            var trigger = skill.PendingTrigger;
            ClearPending(ref skill);

            ecb.CreateFrameEntity(new SkillActivatedEvent
            {
                Character = character,
                Trigger = trigger,
                Skill = skill.Config
            });
        }

        private static void ClearPending(ref Skill skill)
        {
            skill.IsPending = false;
            skill.PendingTrigger = Entity.Null;
        }
    }
}
