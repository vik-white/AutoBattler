using Unity.Collections;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public struct UpgradeConfig : IID
    {
        public uint ID { get; set; }
        public float Health;
        public float Attack;
        public float Defense;
        public float CritChance;
        public float CritValue;
        public FixedList64Bytes<SkillMultiplierData> SkillMultipliers;

        public float GetSkillMultiplier(SkillSlotType slotType)
        {
            for (int i = 0; i < SkillMultipliers.Length; i++)
            {
                if (SkillMultipliers[i].SlotType == slotType) return SkillMultipliers[i].Value;
            }
            return 0f;
        }
    }
}
