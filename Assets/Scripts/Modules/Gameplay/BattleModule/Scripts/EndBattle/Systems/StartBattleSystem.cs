using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup), OrderLast = true)]
    public partial struct StartBattleSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            ecb.CreateSceneEntity(new Battle());
            ecb.Playback(state.EntityManager);
            state.Enabled = false;
        }
    }
}