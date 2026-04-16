using Unity.Entities;

namespace vikwhite.ECS
{
    public struct RenderEntity : IBufferElementData
    {
        public Entity Entity;
        public int MaterialIndex;
    }
}