using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace vikwhite.ECS
{
    public class VFXConfigsAuthoring: MonoBehaviour
    {
        public Material FlashMaterial;
    }

    public class VFXConfigsAuthoringBaker : Baker<VFXConfigsAuthoring>
    {
        public override void Bake(VFXConfigsAuthoring authoring) {
            var entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
            var entity = GetEntity(TransformUsageFlags.None);
            var newFlashMaterialInfo = new MaterialMeshInfo { MaterialID = entitiesGraphicsSystem.RegisterMaterial(authoring.FlashMaterial) };
            AddComponent(entity, new VFXConfig { FlashMaterial = newFlashMaterialInfo.Material });
        }
    }
}