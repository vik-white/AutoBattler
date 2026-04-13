using Unity.Entities;
using Unity.Rendering;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(DamageEffectSystem))]
    public partial struct CreateCharacterFlashSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var target in SystemAPI.Query<RefRO<Target>>().WithAny<DamageEffect>())
            {
                var character = target.ValueRO.Value;
                if(!SystemAPI.HasComponent<Enemy>(character)) return;
                
                if(!SystemAPI.HasComponent<Flash>(character)) ecb.AddComponent<Flash>(character);
                else ecb.SetComponent(character, new Flash { Value = 0 });

                var renderEntity = SystemAPI.GetComponent<RenderEntity>(character);
                var characterMaterialInfo = SystemAPI.GetComponent<MaterialMeshInfo>(renderEntity.Entity);
                characterMaterialInfo.Material = SystemAPI.GetSingleton<VFXConfig>().FlashMaterial;
                ecb.SetComponent(renderEntity.Entity, characterMaterialInfo);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}