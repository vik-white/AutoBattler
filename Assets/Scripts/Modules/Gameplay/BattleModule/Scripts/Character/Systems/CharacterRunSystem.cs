using Rukhanka;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct CharacterRunSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var (transform, previous, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PreviousPosition>>().WithAll<Character>().WithEntityAccess())
            {
                var animator = SystemAPI.GetBuffer<AnimatorControllerParameterComponent>(entity);
                var param = animator[(int)AnimationID.Running];
                param.BoolValue = transform.ValueRO.Position.y < 0.001f && math.length(transform.ValueRO.Position - previous.ValueRO.Value) > 0.01f;
                animator[(int)AnimationID.Running] = param;
            }
        }
    }
}