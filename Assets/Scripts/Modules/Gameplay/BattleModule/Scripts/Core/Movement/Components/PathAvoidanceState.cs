using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct PathAvoidanceState : IComponentData
    {
        public Entity Obstacle;
        public float SideSign;
        public float BlockedTime;
        public float DetourTime;
        public float3 Waypoint;
        public bool HasWaypoint;
    }
}
