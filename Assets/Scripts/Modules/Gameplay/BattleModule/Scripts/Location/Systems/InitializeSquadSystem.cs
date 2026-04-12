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
                var squadConfig = SystemAPI.GetSingletonBuffer<SquadConfig>();
                float spacing = 1.5f;
                float offset = (squadConfig.Length - 1) * spacing / 2f;
                for (int i = 0; i < squadConfig.Length; i++)
                    ecb.CreateFrameEntity(new CreateCharacter
                    {
                        ID = squadConfig[i].ID, 
                        Position = new float3(-4, 0, i * spacing - offset)
                    });
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}