using System;

namespace vikwhite.ECS
{
    [Serializable]
    public struct SkillSlotData<T> where T : unmanaged
    {
        public SkillSlotType Type;
        public T Value;
    }
}
