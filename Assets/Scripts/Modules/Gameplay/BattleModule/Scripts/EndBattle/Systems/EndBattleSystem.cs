using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct EndBattleSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (_, battle) in SystemAPI.Query<RefRO<Battle>>().WithEntityAccess())
            {
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
                    ecb.DestroyEntity(battle);
                }
                else if (!hasAliveCharacters)
                {
                    ecb.CreateFrameEntity(new DefeatBattleEvent());
                    ecb.DestroyEntity(battle);
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}