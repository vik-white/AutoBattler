using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public partial struct SpawnCharacterSystem : ISystem
    {
        public void OnCreate(ref SystemState state) => state.Enabled = false;

        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            
            ecb.CreateFrameEntity(new CreateCharacter{ Position = new float3(0,0,0)});
            
            ecb.Playback(state.EntityManager);
            state.Enabled = false;
        }
    }
}