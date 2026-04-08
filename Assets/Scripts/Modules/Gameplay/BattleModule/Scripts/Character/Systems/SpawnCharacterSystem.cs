using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public partial struct SpawnCharacterSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false;
            state.RequireForUpdate<CharacterConfig>();
            
        }

        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            
            var config = SystemAPI.GetSingletonBuffer<CharacterConfig>()[0];
            ecb.CreateFrameEntity(new CreateCharacter{Config = config, Position = new float3(Random.Range(-3f, 3f), 2, Random.Range(-3f, 3f))});
            
            ecb.Playback(state.EntityManager);
            state.Enabled = false;
        }
    }
}