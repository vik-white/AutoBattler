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
            foreach (var (transform, moveDistance, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<MoveDistance>>().WithAll<Character>().WithEntityAccess())
            {
                var isNearTarget = true;
                var characterID = SystemAPI.GetComponent<Character>(entity).ID;
                var characterConfig = SystemAPI.GetSingletonBuffer<CharacterConfig>().Get(characterID);
                var isHaveAbility = characterConfig.Abilities.Length != 0;
                var isHaveTarget = SystemAPI.HasComponent<Target>(entity);
                if (isHaveAbility && isHaveTarget)
                {
                    var ability = characterConfig.Abilities[0];
                    var abilityRadius = SystemAPI.GetSingletonBuffer<AbilityLevelsConfig>().Get(ability.ID).Levels.Value.Array[ability.Level].Radius;
                    if (abilityRadius > 0)
                    {
                        var target = SystemAPI.GetComponent<Target>(entity).Value;
                        var targetID = SystemAPI.GetComponent<Character>(target).ID;
                        var targetPosition = SystemAPI.GetComponent<LocalTransform>(target).Position;
                        var distanceToTarget = math.length(targetPosition - transform.ValueRO.Position);
                        var characterColliderRadius = characterConfig.ColliderRadius;
                        var targetColliderRadius = SystemAPI.GetSingletonBuffer<CharacterConfig>().Get(targetID).ColliderRadius;
                        isNearTarget = distanceToTarget <= abilityRadius + characterColliderRadius + targetColliderRadius;
                    }
                }
                
                var isGrounded = transform.ValueRO.Position.y < 0.001f;
                
                var animator = SystemAPI.GetBuffer<AnimatorControllerParameterComponent>(entity);
                var param = animator[(int)AnimationID.Running];
                param.BoolValue = !isNearTarget && isGrounded && moveDistance.ValueRO.Value > 0.01f;
                animator[(int)AnimationID.Running] = param;
            }
        }
    }
}