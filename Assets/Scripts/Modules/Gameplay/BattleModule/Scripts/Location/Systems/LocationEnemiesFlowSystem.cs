using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct LocationEnemiesFlowSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var request in SystemAPI.Query<RefRW<LocationEnemiesFlow>>())
            {
                var locationConfig = SystemAPI.GetSingletonBuffer<LocationFlowConfig>().Get(request.ValueRO.ID);
                Debug.Log("LocationEnemiesFlowSystem");
            }
            ecb.Playback(state.EntityManager);
        }
    }
}