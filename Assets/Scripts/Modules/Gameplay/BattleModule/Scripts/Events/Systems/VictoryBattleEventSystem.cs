using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct VictoryBattleEventSystem : ISystem
    {
        public static Action<VictoryBattleEvent> OnExecute;

        public void OnUpdate(ref SystemState state) {
            foreach (var evt in SystemAPI.Query<VictoryBattleEvent>()) OnExecute?.Invoke(evt);
        }
    }
}