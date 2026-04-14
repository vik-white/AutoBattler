using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Aggro : IComponentData
    {
        public Entity Provider;
        public float TimeLeft;
    }
}