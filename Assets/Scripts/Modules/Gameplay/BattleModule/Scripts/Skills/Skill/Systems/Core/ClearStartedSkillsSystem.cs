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
                starterSkills.Clear();
        }
    }
}
