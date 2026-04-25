using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using vikwhite.Data;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(AOEEffectSystem))]
    public partial struct CreateVFXSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>())
            {
                CreateVFX(ref state, ecb, request.ValueRO.Ability, request.ValueRO.Target, request.ValueRO.Provider);
            }
            foreach (var request in SystemAPI.Query<RefRO<CreateStatChange>>())
            {
                CreateVFX(ref state, ecb, request.ValueRO.Ability, request.ValueRO.Target, request.ValueRO.Provider);
            }
            ecb.Playback(state.EntityManager);
        }

        private void CreateVFX(
            ref SystemState state,
            EntityCommandBuffer ecb,
            BlobAssetReference<AbilityConfig> ability,
            Entity targetEntity,
            Entity providerEntity
            )
        {
            var config = ability.Value;
            if(config.VFXPrefab == 0) return;

            var characterPosition = SystemAPI.GetComponent<LocalTransform>(targetEntity).Position;
            var characterConfig = SystemAPI.GetComponent<Character>(targetEntity).GetConfig();
            var position = characterPosition;
            if (config.VFXSpawn == VFXSpawnType.Forward)
            {
                position = new float3(0, 0.5f, 0);
                if (targetEntity != providerEntity)
                {
                    var providerPosition = SystemAPI.GetComponent<LocalTransform>(providerEntity).Position;
                    var colliderRadius = characterConfig.ColliderRadius;
                    var direction = math.normalize(providerPosition - characterPosition) * colliderRadius;
                    position = characterPosition + new float3(direction.x, 0.5f, direction.z);
                }
            }else if(config.VFXSpawn == VFXSpawnType.Top){
                position += new float3(0, characterConfig.Scale, 0);
            }

            ecb.CreateFrameEntity(new CreatePrefabEvent { ID = config.VFXPrefab, Position = position });
            ecb.CreateFrameEntity(new Animation { Character = targetEntity, Type = AnimationType.Reaction, Speed = 1 });
        }
    }
}
