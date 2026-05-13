using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Entities;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateAfter(typeof(SkillSystem))]
    public partial struct SkillAnimationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (events, skills, entity) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>, DynamicBuffer<Skill>>().WithEntityAccess())
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
                    else if (evnt.nameHash == "End".CalculateHash32())
                    {
                        if (state.EntityManager.HasComponent<MovementLock>(entity))
                            ecb.RemoveComponent<MovementLock>(entity);
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
