using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(AnimationSystemGroup))]
    public partial struct CharacterAnimationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var animation in SystemAPI.Query<RefRO<Animation>>())
            {
                var animator = SystemAPI.GetBuffer<AnimatorControllerParameterComponent>(animation.ValueRO.Character);
                var param = animator[(int)animation.ValueRO.ID];
                param.SetTrigger();
                animator[(int)animation.ValueRO.ID] = param;
            }
        }
    }
}