using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct ResurrectCharacterEventSystem : ISystem
    {
        public static Action<ResurrectCharacterEvent> OnExecute;

        public void OnUpdate(ref SystemState state)
        {
            foreach (var evt in SystemAPI.Query<ResurrectCharacterEvent>())
                OnExecute?.Invoke(evt);
        }
    }
}
