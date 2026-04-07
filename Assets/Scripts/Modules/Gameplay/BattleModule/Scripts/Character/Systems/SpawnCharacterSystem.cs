using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    public partial struct SpawnCharacterSystem : ISystem
    {
        public void OnCreate(ref SystemState state) => state.Enabled = false;

        public void OnUpdate(ref SystemState state) {
            Debug.Log("SpawnCharacterSystem");
            var ecb = state.World.GetExistingSystemManaged<BeginSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
            state.Enabled = false;
        }
    }
}