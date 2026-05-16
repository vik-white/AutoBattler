using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateBefore(typeof(ActivateSkillSystem))]
    public partial struct ClearPendingSkillsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var pendingSkills in SystemAPI.Query<DynamicBuffer<PendingSkill>>().WithAll<Dead>())
                pendingSkills.Clear();
        }
    }
}
