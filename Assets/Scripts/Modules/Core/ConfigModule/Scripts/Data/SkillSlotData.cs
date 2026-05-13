using System;

namespace vikwhite.ECS
{
    [Serializable]
    public struct SkillSlotData
    {
        public SkillSlotType SlotType;
        public uint ID;
    }

    [Serializable]
    public struct SkillMultiplierData
    {
        public SkillSlotType SlotType;
        public float Value;
    }

    [Serializable]
    public struct SkillUnlockData
    {
        public SkillSlotType SlotType;
        public int Level;
    }
}
