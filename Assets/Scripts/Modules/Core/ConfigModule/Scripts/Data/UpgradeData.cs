using System;

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
        float SkillActive { get; }
        float SkillPassive1 { get; }
        float SkillPassive2 { get; }
        float SkillMeta1 { get; }
        float SkillMeta2 { get; }
        float SkillMeta3 { get; }
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
        public float SkillActive;
        public float SkillPassive1;
        public float SkillPassive2;
        public float SkillMeta1;
        public float SkillMeta2;
        public float SkillMeta3;
        
        string IUpgradeData.ID => ID;
        float IUpgradeData.Attack => Attack;
        float IUpgradeData.Health => Health;
        float IUpgradeData.Defense => Defense;
        float IUpgradeData.CritChance => CritChance;
        float IUpgradeData.CritValue => CritValue;
        float IUpgradeData.SkillActive => SkillActive;
        float IUpgradeData.SkillPassive1 => SkillPassive1;
        float IUpgradeData.SkillPassive2 => SkillPassive2;
        float IUpgradeData.SkillMeta1 => SkillMeta1;
        float IUpgradeData.SkillMeta2 => SkillMeta2;
        float IUpgradeData.SkillMeta3 => SkillMeta3;
    }
}