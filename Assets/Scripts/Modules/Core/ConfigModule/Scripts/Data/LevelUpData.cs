using System;

namespace vikwhite.Data
{
    public interface ILevelUpData
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
    public class LevelUpData : ILevelUpData
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
        
        string ILevelUpData.ID => ID;
        float ILevelUpData.Attack => Attack;
        float ILevelUpData.Health => Health;
        float ILevelUpData.Defense => Defense;
        float ILevelUpData.CritChance => CritChance;
        float ILevelUpData.CritValue => CritValue;
        float ILevelUpData.SkillActive => SkillActive;
        float ILevelUpData.SkillPassive1 => SkillPassive1;
        float ILevelUpData.SkillPassive2 => SkillPassive2;
        float ILevelUpData.SkillMeta1 => SkillMeta1;
        float ILevelUpData.SkillMeta2 => SkillMeta2;
        float ILevelUpData.SkillMeta3 => SkillMeta3;
    }
}