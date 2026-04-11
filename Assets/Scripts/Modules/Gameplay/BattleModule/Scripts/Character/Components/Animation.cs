using Unity.Entities;

namespace vikwhite.ECS
{
    public enum AnimationID
    {
        None = -1,
        Idle = 0,
        Attack = 1,
        Dead = 2,
        Running = 3,
    }
    
    public struct Animation : IComponentData
    {
        public Entity Character;
        public AnimationID ID;
    }
}