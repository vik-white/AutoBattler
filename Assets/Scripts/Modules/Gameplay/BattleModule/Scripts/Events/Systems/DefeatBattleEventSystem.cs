using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct DefeatBattleEventSystem : ISystem
    {
        public static Action<DefeatBattleEvent> OnExecute;

        public void OnUpdate(ref SystemState state) {
            foreach (var evt in SystemAPI.Query<DefeatBattleEvent>()) OnExecute?.Invoke(evt);
        }
    }
}