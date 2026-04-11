using Unity.Entities;

namespace vikwhite.ECS
{
    public struct RenderEntity : IComponentData
    {
        public Entity Entity;
        public int Material;
    }
}