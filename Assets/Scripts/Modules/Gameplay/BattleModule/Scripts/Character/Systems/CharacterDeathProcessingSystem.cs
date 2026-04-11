using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(DeadSystemGroup))]
    public partial struct CharacterDeathProcessingSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (events, entity) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>>().WithAll<Dead>().WithEntityAccess())
            {
                foreach (var evnt in events)
                {
                    if (evnt.nameHash == "DeadEvent".CalculateHash32())
                        ecb.AddComponent<Destroy>(entity);
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}