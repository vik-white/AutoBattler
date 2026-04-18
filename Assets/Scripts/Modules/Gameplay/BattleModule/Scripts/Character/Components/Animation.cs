using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Animation : IComponentData
    {
        public Entity Character;
        public AnimationType Type;
        public float Speed;
    }
}