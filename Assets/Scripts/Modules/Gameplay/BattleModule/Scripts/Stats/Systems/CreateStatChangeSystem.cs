using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateStatChangeSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateStatChange>>())
            {
                var statChange = ecb.CreateSceneEntity();
                ecb.AddComponent(statChange,  new StatChange {
                    Target = request.ValueRO.Target,
                    Type = request.ValueRO.Data.Type,
                    Value = request.ValueRO.Data.Value
                });
                ecb.AddComponent(statChange, new DestroyTimer{ Time = request.ValueRO.Data.Duration });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}