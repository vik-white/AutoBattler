using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(AnimationSystemGroup))]
    public partial struct CharacterAnimationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var skillStartedEvent in SystemAPI.Query<RefRO<SkillStartedEvent>>())
            {
                var skillConfig = skillStartedEvent.ValueRO.Skill.Value;
                if (!SkillHandler.HasActivationAnimation(skillConfig)) continue;

                PlayAnimation(ref state, skillStartedEvent.ValueRO.Character, skillConfig.Animation, skillStartedEvent.ValueRO.Speed);
            }

            foreach (var deadEvent in SystemAPI.Query<RefRO<DeadCharacterEvent>>())
                PlayAnimation(ref state, deadEvent.ValueRO.Character, AnimationType.Dead, 1f);

            foreach (var createEffectEvent in SystemAPI.Query<RefRO<CreateEffectEvent>>())
                PlayAnimation(ref state, createEffectEvent.ValueRO.Target, AnimationType.Reaction, 1f);
        }

        private void PlayAnimation(ref SystemState state, Entity character, AnimationType animation, float speed)
        {
            var animator = SystemAPI.GetBuffer<AnimatorControllerParameterComponent>(character);
            var param = animator[(int)animation];
            param.SetTrigger();
            animator[(int)animation] = param;

            var animatorLayers = SystemAPI.GetBuffer<AnimatorControllerLayerComponent>(character);
            var layer = animatorLayers[0];
            layer.speed = speed;
            animatorLayers[0] = layer;
        }
    }
}
