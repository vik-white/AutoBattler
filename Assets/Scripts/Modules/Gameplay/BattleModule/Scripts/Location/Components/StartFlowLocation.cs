using Unity.Entities;

namespace vikwhite.ECS
{
    public struct LocationEnemiesFlow : IComponentData
    {
        public uint ID;
        public float Cooldown;
    }
}