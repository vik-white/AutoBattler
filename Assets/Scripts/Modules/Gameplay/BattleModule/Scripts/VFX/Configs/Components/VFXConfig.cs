using Unity.Entities;

namespace vikwhite.ECS
{
    public struct VFXConfig : IComponentData
    {
        public Entity Flash;
        public int FlashMaterial;
    }
}