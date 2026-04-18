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
                var param = animator[(int)animation.ValueRO.Type];
                param.SetTrigger();
                animator[(int)animation.ValueRO.Type] = param;

                /*var animatorLayers = SystemAPI.GetBuffer<AnimatorControllerLayerComponent>(animation.ValueRO.Character);
                var layer = animatorLayers[0];
                layer.speed = animation.ValueRO.Speed;
                animatorLayers[0] = layer;*/
            }
        }
    }
}