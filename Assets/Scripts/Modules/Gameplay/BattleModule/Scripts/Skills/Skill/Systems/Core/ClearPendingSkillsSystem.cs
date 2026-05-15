using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateBefore(typeof(ActivateAnimatedSkillSystem))]
    public partial struct ClearPendingSkillsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var skills in SystemAPI.Query<DynamicBuffer<Skill>>().WithAll<Dead>())
                ClearPendingSkills(skills);
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
