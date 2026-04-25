using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct LocationEnemiesFlowSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var time = SystemAPI.GetSingleton<Time>();
            var dt = time.DeltaTime;
            var flowConfigs = SystemAPI.GetSingleton<LocationFlowConfigsBlob>().Value;
            foreach (var request in SystemAPI.Query<RefRW<LocationEnemiesFlow>>())
            {
                request.ValueRW.Cooldown -= dt;
                if(request.ValueRO.Cooldown > 0) continue;
                
                ref var locationConfig = ref flowConfigs.Get(request.ValueRO.ID);
                LocationFlowStepData step = default;
                for (int i = 0; i < locationConfig.Steps.Length; i++)
                {
                    if (locationConfig.Steps[i].Time > time.TotalTime) break;
                    step = locationConfig.Steps[i];
                }
                request.ValueRW.Cooldown = step.SpawnInterval;
                
                int enemyCount = state.EntityManager.CreateEntityQuery(typeof(Enemy)).CalculateEntityCount();
                if(enemyCount >= step.Count) continue;
                    
                ecb.CreateFrameEntity(new CreateCharacter
                {
                    ID = step.Enemies[UnityEngine.Random.Range(0, step.Enemies.Length)], 
                    Position = new float3(8, 0, UnityEngine.Random.Range(-4f, 4f)), 
                    IsEnemy = true
                });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
