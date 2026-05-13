using System;

namespace vikwhite.ECS
{
    [Serializable]
    public struct SkillSlotData
    {
        public SkillType Type;
        public uint ID;
    }

    [Serializable]
    public struct SkillMultiplierData
    {
        public SkillType Type;
        public float Value;
    }

    [Serializable]
    public struct SkillUnlockData
    {
        public SkillType Type;
        public int Level;
    }
}
