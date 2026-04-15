using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct CreatePrefabEventSystem : ISystem
    {
        public static Action<CreatePrefabEvent> OnExecute;

        public void OnUpdate(ref SystemState state) {
            foreach (var evt in SystemAPI.Query<CreatePrefabEvent>()) OnExecute?.Invoke(evt);
        }
    }
}