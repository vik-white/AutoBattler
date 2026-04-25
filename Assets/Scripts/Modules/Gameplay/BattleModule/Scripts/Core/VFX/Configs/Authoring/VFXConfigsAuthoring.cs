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
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new VFXConfig { FlashMaterial = authoring.FlashMaterial });
        }
    }
}