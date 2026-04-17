using Rukhanka.Toolbox;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(AOEEffectSystem))]
    public partial struct CreateEffectVFXSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var characterConfig = SystemAPI.GetSingletonBuffer<CharacterConfig>();
            var abilityLevelsConfig = SystemAPI.GetSingletonBuffer<AbilityLevelsConfig>();
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>())
            {
                var ability = abilityLevelsConfig.Get(request.ValueRO.Ability.ID).Levels.Value.Array[request.ValueRO.Ability.Level];
                if(ability.VFXPrefab == 0) continue;

                var characterPosition = SystemAPI.GetComponent<LocalTransform>(request.ValueRO.Target).Position;
                var position = characterPosition;
                if (ability.VFXSpawn == VFXSpawnType.Forward)
                {
                    position = new float3(0, 0.5f, 0);
                    if (request.ValueRO.Target != request.ValueRO.Provider)
                    {
                        var providerPosition = SystemAPI.GetComponent<LocalTransform>(request.ValueRO.Provider).Position;
                        var id = SystemAPI.GetComponent<Character>(request.ValueRO.Target).ID;
                        var config = characterConfig.Get(id);
                        var colliderRadius = config.ColliderRadius;
                        var direction = math.normalize(providerPosition - characterPosition) * colliderRadius;
                        position = characterPosition + new float3(direction.x, 0.5f, direction.z);
                    }
                }

                ecb.CreateFrameEntity(new CreatePrefabEvent { ID = ability.VFXPrefab, Position = position });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}