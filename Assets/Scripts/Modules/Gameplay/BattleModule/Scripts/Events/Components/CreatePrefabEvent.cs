using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreatePrefabEvent : IComponentData
    {
        public uint ID;
        public float3 Position;
    }
}