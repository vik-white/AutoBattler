using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public partial struct InitializeSquadSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (request, entity) in SystemAPI.Query<RefRO<InitializeSquad>>().WithEntityAccess())
            {
                for (int i = 0; i < request.ValueRO.Value.Length; i++)
                {
                    var id = request.ValueRO.Value[i];
                    if(id == 0) continue;
                    ecb.CreateFrameEntity(new CreateCharacter
                    {
                        ID = id, 
                        Position = GetPosition(i)
                    });
                }
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }

        private float3 GetPosition(int index)
        {
            switch (index)
            {
                case 0: return new float3(-4, 0, 1f);
                case 1: return new float3(-4, 0, -1f);
                case 2: return new float3(-5, 0, 2);
                case 3: return new float3(-5, 0, 0);
                case 4: return new float3(-5, 0, -2);
                default: return float3.zero;
            }
        }
    }
}