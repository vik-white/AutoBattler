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
            foreach (var (events, skills) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>, DynamicBuffer<Skill>>())
            {
                foreach (var evnt in events)
                {
                    if (evnt.nameHash == "Attack".CalculateHash32())
                    {
                        for (int i = 0; i < skills.Length; i++)
                        {
                            ref var skill = ref skills.ElementAt(i);
                            skill.IsActivated = false;
                            if (skill.IsAnimating)
                            {
                                skill.IsActivated = true;
                                skill.IsAnimating = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
