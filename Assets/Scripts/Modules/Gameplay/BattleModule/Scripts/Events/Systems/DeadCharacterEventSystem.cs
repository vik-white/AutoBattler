using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct DeadCharacterEventSystem : ISystem
    {
        public static Action<DeadCharacterEvent> OnExecute;

        public void OnUpdate(ref SystemState state) {
            foreach (var evt in SystemAPI.Query<DeadCharacterEvent>()) OnExecute?.Invoke(evt);
        }
    }
}