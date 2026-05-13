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
        IReadOnlyDictionary<SkillType, float> SkillMultipliers { get; }
        float GetSkillMultiplier(SkillType slot);
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

        private Dictionary<SkillType, float> _skillMultipliers;

        public IReadOnlyDictionary<SkillType, float> SkillMultipliers => _skillMultipliers ??= BuildSkillMultipliers();

        string IUpgradeData.ID => ID;
        float IUpgradeData.Attack => Attack;
        float IUpgradeData.Health => Health;
        float IUpgradeData.Defense => Defense;
        float IUpgradeData.CritChance => CritChance;
        float IUpgradeData.CritValue => CritValue;
        IReadOnlyDictionary<SkillType, float> IUpgradeData.SkillMultipliers => SkillMultipliers;

        public float GetSkillMultiplier(SkillType slot) => SkillMultipliers.TryGetValue(slot, out var value) ? value : 0f;

        private Dictionary<SkillType, float> BuildSkillMultipliers() => new()
        {
            { SkillType.Active, SkillActive },
            { SkillType.Passive1, SkillPassive1 },
            { SkillType.Passive2, SkillPassive2 },
            { SkillType.Meta1, SkillMeta1 },
            { SkillType.Meta2, SkillMeta2 },
            { SkillType.Meta3, SkillMeta3 },
        };
    }
}
