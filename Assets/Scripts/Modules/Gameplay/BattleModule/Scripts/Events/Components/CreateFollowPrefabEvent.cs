using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateFollowPrefabEvent : IComponentData
    {
        public uint ID;
        public float3 Position;
        public Entity Entity;
    }
}