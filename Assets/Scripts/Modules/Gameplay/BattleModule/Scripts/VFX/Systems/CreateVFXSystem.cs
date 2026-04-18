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
            var characterConfig = SystemAPI.GetSingletonBuffer<CharacterConfig>();
            var abilityLevelsConfig = SystemAPI.GetSingletonBuffer<AbilityLevelsConfig>();
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>())
            {
                CreateVFX(ref state, ecb, characterConfig, abilityLevelsConfig, request.ValueRO.Ability, request.ValueRO.Target, request.ValueRO.Provider);
            }
            foreach (var request in SystemAPI.Query<RefRO<CreateStatChange>>())
            {
                CreateVFX(ref state, ecb, characterConfig, abilityLevelsConfig, request.ValueRO.Ability, request.ValueRO.Target, request.ValueRO.Provider);
            }
            ecb.Playback(state.EntityManager);
        }

        private void CreateVFX(
            ref SystemState state, 
            EntityCommandBuffer ecb, 
            DynamicBuffer<CharacterConfig> characterConfig, 
            DynamicBuffer<AbilityLevelsConfig> abilityLevelsConfig, 
            AbilityLevelData abilityLevelData, 
            Entity targetEntity,
            Entity providerEntity 
            )
        {
            var ability = abilityLevelsConfig.Get(abilityLevelData.ID).Levels.Value.Array[abilityLevelData.Level];
            if(ability.VFXPrefab == 0) return;

            var characterPosition = SystemAPI.GetComponent<LocalTransform>(targetEntity).Position;
            var id = SystemAPI.GetComponent<Character>(targetEntity).ID;
            var config = characterConfig.Get(id);
            var position = characterPosition;
            if (ability.VFXSpawn == VFXSpawnType.Forward)
            {
                position = new float3(0, 0.5f, 0);
                if (targetEntity != providerEntity)
                {
                    var providerPosition = SystemAPI.GetComponent<LocalTransform>(providerEntity).Position;
                    var colliderRadius = config.ColliderRadius;
                    var direction = math.normalize(providerPosition - characterPosition) * colliderRadius;
                    position = characterPosition + new float3(direction.x, 0.5f, direction.z);
                }
            }else if(ability.VFXSpawn == VFXSpawnType.Top){
                position += new float3(0, config.Scale, 0);
            }

            ecb.CreateFrameEntity(new CreatePrefabEvent { ID = ability.VFXPrefab, Position = position });
            ecb.CreateFrameEntity(new Animation { Character = targetEntity, Type = AnimationType.Reaction, Speed = 1 });
        }
    }
}