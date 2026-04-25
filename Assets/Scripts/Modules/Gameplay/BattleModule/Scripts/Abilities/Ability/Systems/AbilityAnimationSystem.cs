using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Entities;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateAfter(typeof(AbilitySystem))]
    public partial struct AbilityAnimationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (events, abilities) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>, DynamicBuffer<Ability>>())
            {
                foreach (var evnt in events)
                {
                    if (evnt.nameHash == "Attack".CalculateHash32())
                    {
                        for (int i = 0; i < abilities.Length; i++)
                        {
                            ref var ability = ref abilities.ElementAt(i);
                            ability.IsActivate = false;
                            if (ability.IsAnimation)
                            {
                                ability.IsActivate = true;
                                ability.IsAnimation = false;
                            }
                        }
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}