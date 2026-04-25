using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace vikwhite.ECS
{
    public struct VFXConfig : IComponentData
    {
        public int FlashMaterialIndex;
        public UnityObjectRef<Material> FlashMaterial;
    }
}