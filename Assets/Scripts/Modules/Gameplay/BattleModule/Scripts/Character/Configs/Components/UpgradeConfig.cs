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
        public FixedList64Bytes<SkillSlotData<float>> SkillMultipliers;

        public float GetSkillMultiplier(SkillSlotType slotType) => SkillMultipliers.Get(slotType);

        public float GetStatMultiplier(StatType stat) => stat switch
        {
            StatType.Attack => Attack,
            StatType.Defense => Defense,
            StatType.Health => Health,
            StatType.CritChance => CritChance,
            StatType.CritValue => CritValue,
            _ => 0f,
        };
    }
}
