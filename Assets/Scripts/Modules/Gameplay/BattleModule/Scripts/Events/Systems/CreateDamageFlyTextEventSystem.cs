using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct CreateDamageFlyTextEventSystem : ISystem
    {
        public static Action<CreateDamageFlyTextEvent> OnExecute;

        public void OnUpdate(ref SystemState state) {
            foreach (var evt in SystemAPI.Query<CreateDamageFlyTextEvent>()) OnExecute?.Invoke(evt);
        }
    }
}
