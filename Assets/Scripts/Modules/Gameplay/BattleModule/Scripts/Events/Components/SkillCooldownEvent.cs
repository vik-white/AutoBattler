using Unity.Entities;

namespace vikwhite.ECS
{
    public struct SkillCooldownEvent : IComponentData
    {
        public Entity Character;
        public uint SkillID;
    }
}
