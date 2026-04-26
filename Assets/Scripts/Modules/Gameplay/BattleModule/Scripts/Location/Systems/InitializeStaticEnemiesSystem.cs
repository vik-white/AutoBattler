using Unity.Entities;
using Unity.Mathematics;
using vikwhite.Utils;

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
            var hexPositionsConfigs = SystemAPI.GetSingleton<HexPositionsConfigsBlob>().Value;
            foreach (var (request, entity) in SystemAPI.Query<RefRW<InitializeStaticEnemies>>().WithEntityAccess())
            {
                ref var locationConfig = ref staticConfigs.Get(request.ValueRO.ID);
                var hexPositionsConfig = hexPositionsConfigs.Get(locationConfig.HexPositions);
                for (int i = 0; i < locationConfig.Enemies.Length; i++)
                    ecb.CreateFrameEntity(new CreateCharacter
                    {
                        ID = locationConfig.Enemies[i].ID, 
                        Level = locationConfig.Enemies[i].Level, 
                        Position = GetPosition(i, hexPositionsConfig),
                        IsEnemy = true
                    });
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }
        
        private float3 GetPosition(int index, HexPositionsConfig hexPositionsConfig)
        {
            switch (index)
            {
                case 0: return HexCoordinatesHandler.AxialToWorld(hexPositionsConfig.C1);
                case 1: return HexCoordinatesHandler.AxialToWorld(hexPositionsConfig.C2);
                case 2: return HexCoordinatesHandler.AxialToWorld(hexPositionsConfig.C3);
                case 3: return HexCoordinatesHandler.AxialToWorld(hexPositionsConfig.C4);
                case 4: return HexCoordinatesHandler.AxialToWorld(hexPositionsConfig.C5);
                default: return float3.zero;
            }
        }
    }
}
