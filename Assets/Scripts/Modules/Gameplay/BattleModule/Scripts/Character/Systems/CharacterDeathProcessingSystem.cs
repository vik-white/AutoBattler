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
            var pendingResurrections = SystemAPI.GetComponentLookup<PendingResurrection>(true);
            var deadEventHash = "DeadEvent".CalculateHash32();

            foreach (var (events, dead, entity) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>, RefRW<Dead>>().WithEntityAccess())
            {
                foreach (var evnt in events)
                {
                    if (evnt.nameHash != deadEventHash) continue;

                    dead.ValueRW.AnimationCompleted = true;
                    if (!pendingResurrections.HasComponent(entity))
                        ecb.AddComponent<Destroy>(entity);
                    break;
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
