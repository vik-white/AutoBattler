using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateVFXSystem))]
    public partial struct CreateEffectImpulseSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>())
            {
                var abilityConfig = request.ValueRO.Ability.Value;
                var impulse = float3.zero;
                if (abilityConfig.ImpulseUp > 0) impulse += new float3(0, abilityConfig.ImpulseUp * 2, 0);
                if (abilityConfig.ImpulseProvider > 0)
                {
                    var targetPosition = SystemAPI.GetComponent<LocalTransform>(request.ValueRO.Target).Position;
                    var providerPosition = SystemAPI.GetComponent<LocalTransform>(request.ValueRO.Provider).Position;
                    var direction = math.normalize(targetPosition - providerPosition);
                    impulse += direction * abilityConfig.ImpulseProvider * 2;
                }
                if (impulse.x == 0 && impulse.y == 0 && impulse.z == 0) continue;
                ecb.AddComponent(request.ValueRO.Target, new Impulse { Value = impulse });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
