using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct CreateFollowPrefabEventSystem : ISystem
    {
        public static Action<CreateFollowPrefabEvent> OnExecute;

        public void OnUpdate(ref SystemState state) {
            foreach (var evt in SystemAPI.Query<CreateFollowPrefabEvent>()) OnExecute?.Invoke(evt);
        }
    }
}