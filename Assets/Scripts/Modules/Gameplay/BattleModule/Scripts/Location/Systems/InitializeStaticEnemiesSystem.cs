using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public partial struct InitializeStaticEnemiesSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (request, entity) in SystemAPI.Query<RefRW<InitializeStaticEnemies>>().WithEntityAccess())
            {
                var locationConfig = SystemAPI.GetSingletonBuffer<LocationStaticConfig>().Get(request.ValueRO.ID);
                float spacing = 1.5f;
                float offset = (locationConfig.Enemies.Length - 1) * spacing / 2f;
                for (int i = 0; i < locationConfig.Enemies.Length; i++)
                    ecb.CreateFrameEntity(new CreateCharacter
                    {
                        ID = locationConfig.Enemies[i], 
                        Position = new float3(4, 0, i * spacing - offset), 
                        IsEnemy = true
                    });
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}