using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct VFXConfigInitializeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            var entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
            var configs = SystemAPI.GetSingleton<VFXConfig>();
            var newFlashMaterialInfo = new MaterialMeshInfo { MaterialID = entitiesGraphicsSystem.RegisterMaterial(configs.FlashMaterial) };
            configs.FlashMaterialIndex = newFlashMaterialInfo.Material;
            SystemAPI.SetSingleton(configs);
            state.Enabled = false;
        }
    }
}
