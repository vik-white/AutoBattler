using Unity.Entities;

namespace vikwhite.ECS
{
    public struct ActivateSkillEvent : IComponentData
    {
        public Entity Character;
        public uint SkillID;
    }
}
