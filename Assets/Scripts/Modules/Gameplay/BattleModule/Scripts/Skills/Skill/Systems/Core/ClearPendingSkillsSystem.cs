using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateBefore(typeof(ActivateSkillSystem))]
    public partial struct ClearPendingSkillsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

            foreach (var instantSkills in SystemAPI.Query<DynamicBuffer<SkillInstant>>().WithAll<Dead>())
                instantSkills.Clear();

            foreach (var (_, character) in SystemAPI.Query<RefRO<SkillAnimated>>().WithAll<Dead>().WithEntityAccess())
                ecb.RemoveComponent<SkillAnimated>(character);

            ecb.Playback(state.EntityManager);
        }
    }
}
