using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateBefore(typeof(ActivateSkillSystem))]
    public partial struct ClearStartedSkillsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var starterSkills in SystemAPI.Query<DynamicBuffer<StarterSkill>>().WithAll<Dead>())
            {
                for (var i = starterSkills.Length - 1; i >= 0; i--)
                {
                    if (starterSkills[i].Skill.Value.Trigger != TriggerType.Dead)
                        starterSkills.RemoveAt(i);
                }
            }
        }
    }
}
