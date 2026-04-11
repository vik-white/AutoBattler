using Unity.Entities;
using Unity.Rendering;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public partial struct InitializeVFXSystem: ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false;
            state.RequireForUpdate<VFXConfig>();
        }
        
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var vfxConfigEntity = SystemAPI.GetSingletonEntity<VFXConfig>();
            var vfxConfig = SystemAPI.GetComponent<VFXConfig>(vfxConfigEntity);
            var flashMaterialInfo = SystemAPI.GetComponent<MaterialMeshInfo>(vfxConfig.Flash);
            vfxConfig.FlashMaterial = flashMaterialInfo.Material;
            ecb.SetComponent(vfxConfigEntity, vfxConfig);
            ecb.Playback(state.EntityManager);
            state.Enabled = false;
        }
    }
}