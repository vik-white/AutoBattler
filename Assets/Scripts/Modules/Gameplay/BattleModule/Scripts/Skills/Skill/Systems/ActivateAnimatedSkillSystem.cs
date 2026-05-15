using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateAfter(typeof(SkillCooldownSystem))]
    public partial struct ActivateAnimatedSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var dead = SystemAPI.GetComponentLookup<Dead>(true);
            var attackHash = "Attack".CalculateHash32();

            foreach (var (events, skills, character) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>, DynamicBuffer<Skill>>().WithEntityAccess())
            {
                if (dead.HasComponent(character))
                {
                    ClearPendingSkills(skills);
                    continue;
                }

                if (!HasAttackEvent(events, attackHash)) continue;

                for (int i = 0; i < skills.Length; i++)
                {
                    ref var skill = ref skills.ElementAt(i);
                    if (!skill.IsPending) continue;

                    skill.IsPending = false;
                    if (!skill.Config.IsCreated) continue;

                    ecb.CreateFrameEntity(new SkillActivatedEvent
                    {
                        Character = character,
                        Skill = skill.Config
                    });
                }
            }

            ecb.Playback(state.EntityManager);
        }

        private static bool HasAttackEvent(DynamicBuffer<AnimationEventComponent> events, uint attackHash)
        {
            foreach (var evnt in events)
            {
                if (evnt.nameHash == attackHash)
                    return true;
            }

            return false;
        }

        private static void ClearPendingSkills(DynamicBuffer<Skill> skills)
        {
            for (int i = 0; i < skills.Length; i++)
            {
                ref var skill = ref skills.ElementAt(i);
                skill.IsPending = false;
            }
        }
    }
}
