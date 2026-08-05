using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializeSystemGroup), OrderFirst = true)]
    public partial struct EndBattleSystem : ISystem
    {
        private enum BattleResult
        {
            None,
            Victory,
            Defeat
        }

        private BattleResult _pendingResult;

        public void OnUpdate(ref SystemState state)
        {
            if (SystemAPI.HasSingleton<InitializeSquad>())
            {
                _pendingResult = BattleResult.None;
                return;
            }
            
            bool hasAliveEnemies = false;
            foreach (var _ in SystemAPI.Query<RefRO<Enemy>>().WithNone<Dead>())
            {
                hasAliveEnemies = true;
                break;
            }
            if (!hasAliveEnemies)
            {
                foreach (var _ in SystemAPI.Query<RefRO<PendingResurrection>>().WithAll<Enemy>())
                {
                    hasAliveEnemies = true;
                    break;
                }
            }

            bool hasAliveCharacters = false;
            foreach (var _ in SystemAPI.Query<RefRO<Character>>().WithNone<Enemy, Dead>())
            {
                hasAliveCharacters = true;
                break;
            }
            if (!hasAliveCharacters)
            {
                foreach (var _ in SystemAPI.Query<RefRO<PendingResurrection>>().WithNone<Enemy>())
                {
                    hasAliveCharacters = true;
                    break;
                }
            }

            var result = !hasAliveEnemies
                ? BattleResult.Victory
                : !hasAliveCharacters
                    ? BattleResult.Defeat
                    : BattleResult.None;

            if (result == BattleResult.None)
            {
                _pendingResult = BattleResult.None;
                return;
            }

            if (_pendingResult != result)
            {
                _pendingResult = result;
                return;
            }

            _pendingResult = BattleResult.None;
            state.Enabled = false;
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            if (result == BattleResult.Victory)
                ecb.CreateFrameEntity(new VictoryBattleEvent());
            else
                ecb.CreateFrameEntity(new DefeatBattleEvent());

            ecb.Playback(state.EntityManager);
        }
    }
}
