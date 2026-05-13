using System;
using System.Collections.Generic;

namespace vikwhite.Data
{
    public interface IUpgradeData
    {
        string ID { get; }
        float Health { get; }
        float Attack { get; }
        float Defense { get; }
        float CritChance { get; }
        float CritValue { get; }
        IReadOnlyDictionary<SkillSlotType, float> SkillMultipliers { get; }
        float GetSkillMultiplier(SkillSlotType slotType);
    }

    [Serializable]
    public class UpgradeData : IUpgradeData
    {
        public string ID;
        public float Health;
        public float Attack;
        public float Defense;
        public float CritChance;
        public float CritValue;

        // Stored per-slot for backwards compatibility with the existing asset and Google Sheet columns.
        // Code should access multipliers via the dictionary view below.
        public float SkillActive;
        public float SkillPassive1;
        public float SkillPassive2;
        public float SkillMeta1;
        public float SkillMeta2;
        public float SkillMeta3;

        private Dictionary<SkillSlotType, float> _skillMultipliers;

        public IReadOnlyDictionary<SkillSlotType, float> SkillMultipliers => _skillMultipliers ??= BuildSkillMultipliers();

        string IUpgradeData.ID => ID;
        float IUpgradeData.Attack => Attack;
        float IUpgradeData.Health => Health;
        float IUpgradeData.Defense => Defense;
        float IUpgradeData.CritChance => CritChance;
        float IUpgradeData.CritValue => CritValue;
        IReadOnlyDictionary<SkillSlotType, float> IUpgradeData.SkillMultipliers => SkillMultipliers;

        public float GetSkillMultiplier(SkillSlotType slotType) => SkillMultipliers.TryGetValue(slotType, out var value) ? value : 0f;

        private Dictionary<SkillSlotType, float> BuildSkillMultipliers() => new()
        {
            { SkillSlotType.Active, SkillActive },
            { SkillSlotType.Passive1, SkillPassive1 },
            { SkillSlotType.Passive2, SkillPassive2 },
            { SkillSlotType.Meta1, SkillMeta1 },
            { SkillSlotType.Meta2, SkillMeta2 },
            { SkillSlotType.Meta3, SkillMeta3 },
        };
    }
}
