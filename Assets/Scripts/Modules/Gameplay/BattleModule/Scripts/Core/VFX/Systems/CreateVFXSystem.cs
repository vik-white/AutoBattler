using Unity.Entities;

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
                CreateVisualEvent(ecb, request.ValueRO.Skill, request.ValueRO.Target, request.ValueRO.Provider);
            }
            foreach (var request in SystemAPI.Query<RefRO<CreateStatChange>>())
            {
                CreateVisualEvent(ecb, request.ValueRO.Skill, request.ValueRO.Target, request.ValueRO.Provider);
            }
            ecb.Playback(state.EntityManager);
        }

        private static void CreateVisualEvent(EntityCommandBuffer ecb, BlobAssetReference<SkillConfig> ability, Entity targetEntity, Entity providerEntity)
        {
            if (!ability.IsCreated || ability.Value.VFXPrefab == 0) return;

            ecb.CreateFrameEntity(new SkillEffectVisualEvent
            {
                Skill = ability,
                Target = targetEntity,
                Provider = providerEntity
            });
        }
    }
}
