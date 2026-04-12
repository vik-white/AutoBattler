using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace vikwhite.ECS
{
    public class VFXConfigsAuthoring: MonoBehaviour
    {
        public GameObject Flash;
    }

    public class VFXConfigsAuthoringBaker : Baker<VFXConfigsAuthoring>
    {
        public override void Bake(VFXConfigsAuthoring authoring) {  
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new VFXConfig
            {
                Flash = GetEntity(authoring.Flash, TransformUsageFlags.Dynamic)
            });
        }
    }
}