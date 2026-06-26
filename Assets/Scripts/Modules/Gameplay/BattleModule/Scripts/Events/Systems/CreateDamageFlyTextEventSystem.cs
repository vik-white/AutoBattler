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

    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct StartedSkillEventSystem : ISystem
    {
        public static Action<StartedSkillEvent> OnExecute;

        public void OnUpdate(ref SystemState state) {
            foreach (var evt in SystemAPI.Query<StartedSkillEvent>()) OnExecute?.Invoke(evt);
        }
    }
}
