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
            
            /*var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            foreach (var (transform, entity) in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Dead>().WithEntityAccess())
            {
                transform.ValueRW.Position -= new float3(0, dt * 1f, 0);
                if (transform.ValueRO.Position.y <= -1)
                {
                    ecb.AddComponent<Destroy>(entity);
                }
            }*/
            
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