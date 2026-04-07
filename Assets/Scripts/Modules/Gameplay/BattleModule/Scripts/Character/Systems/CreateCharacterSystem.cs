using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateCharacterSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var request in SystemAPI.Query<RefRO<CreateCharacter>>())
            {
                Debug.Log(request.ValueRO.Position);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}