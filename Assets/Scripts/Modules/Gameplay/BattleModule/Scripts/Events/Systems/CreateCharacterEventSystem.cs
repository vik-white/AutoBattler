using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct CreateCharacterEventSystem : ISystem
    {
        public static Action<CreateCharacterEvent> OnExecute;

        public void OnUpdate(ref SystemState state) {
            foreach (var evt in SystemAPI.Query<CreateCharacterEvent>()) OnExecute?.Invoke(evt);
        }
    }
}