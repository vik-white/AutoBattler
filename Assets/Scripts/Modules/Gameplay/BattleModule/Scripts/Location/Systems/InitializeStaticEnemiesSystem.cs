using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    [UpdateAfter(typeof(InitializeSquadSystem))]
    public partial struct InitializeStaticEnemiesSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var staticConfigs = SystemAPI.GetSingleton<LocationStaticConfigsBlob>().Value;
            foreach (var (request, entity) in SystemAPI.Query<RefRW<InitializeStaticEnemies>>().WithEntityAccess())
            {
                ref var locationConfig = ref staticConfigs.Get(request.ValueRO.ID);
                float spacing = 1.5f;
                float offset = (locationConfig.Enemies.Length - 1) * spacing / 2f;
                for (int i = 0; i < locationConfig.Enemies.Length; i++)
                    ecb.CreateFrameEntity(new CreateCharacter
                    {
                        ID = locationConfig.Enemies[i], 
                        Position = GetPosition(i),
                        IsEnemy = true
                    });
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }
        
        private float3 GetPosition(int index)
        {
            switch (index)
            {
                case 0: return new float3(4, 0, 1f);
                case 1: return new float3(4, 0, -1f);
                case 2: return new float3(5, 0, -2);
                case 3: return new float3(5, 0, 0);
                case 4: return new float3(5, 0, 2);
                default: return float3.zero;
            }
        }
    }
}
