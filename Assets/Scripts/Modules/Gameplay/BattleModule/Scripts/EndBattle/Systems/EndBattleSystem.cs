using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializeSystemGroup), OrderFirst = true)]
    public partial struct EndBattleSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if(SystemAPI.HasSingleton<InitializeSquad>()) return;
            
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            bool hasAliveEnemies = false;
            foreach (var _ in SystemAPI.Query<RefRO<Enemy>>().WithNone<Dead>())
            {
                hasAliveEnemies = true;
                break;
            }

            bool hasAliveCharacters = false;
            foreach (var _ in SystemAPI.Query<RefRO<Character>>().WithNone<Enemy, Dead>())
            {
                hasAliveCharacters = true;
                break;
            }

            if (!hasAliveEnemies)
            {
                ecb.CreateFrameEntity(new VictoryBattleEvent());
            }
            else if (!hasAliveCharacters)
            {
                ecb.CreateFrameEntity(new DefeatBattleEvent());
            }
            ecb.Playback(state.EntityManager);
        }
    }
}