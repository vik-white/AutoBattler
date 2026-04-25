using Rukhanka;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(MovementSystemGroup))]
    public partial struct CharacterRunSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, moveDistance, character, abilities, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<MoveDistance>, RefRO<Character>, DynamicBuffer<Ability>>().WithEntityAccess())
            {
                var isNearTarget = true;
                var characterConfig = character.ValueRO.GetConfig();
                var isHaveAbility = abilities.Length != 0;
                var isHaveTarget = SystemAPI.HasComponent<Target>(entity);
                if (isHaveAbility && isHaveTarget)
                {
                    var abilityRadius = abilities[0].GetConfig().Radius;
                    if (abilityRadius > 0)
                    {
                        var target = SystemAPI.GetComponent<Target>(entity).Value;
                        var targetCharacter = SystemAPI.GetComponent<Character>(target);
                        var targetPosition = SystemAPI.GetComponent<LocalTransform>(target).Position;
                        var distanceToTarget = math.length(targetPosition - transform.ValueRO.Position);
                        var characterColliderRadius = characterConfig.ColliderRadius;
                        var targetColliderRadius = targetCharacter.GetConfig().ColliderRadius;
                        isNearTarget = distanceToTarget <= abilityRadius + characterColliderRadius + targetColliderRadius;
                    }
                }

                var isGrounded = transform.ValueRO.Position.y < 0.001f;

                var animator = SystemAPI.GetBuffer<AnimatorControllerParameterComponent>(entity);
                var param = animator[(int)AnimationType.Running];
                param.BoolValue = !isNearTarget && isGrounded && moveDistance.ValueRO.Value > 0.01f;
                animator[(int)AnimationType.Running] = param;
            }
        }
    }
}
