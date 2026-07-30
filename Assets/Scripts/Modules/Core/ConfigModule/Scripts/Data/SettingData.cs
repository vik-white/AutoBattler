using System;

namespace vikwhite
{
    public interface ISettingData
    {
        int LevelUpPrice { get; }
        int SkillUpPrice { get; }
        int StarUpPrice { get; }
        float BreakthroughMultiply { get; }
        int BreakthroughLevelPeriod { get; }
        int BreakthroughHeroesCount { get; }
        int BreakthroughEssence { get; }
        int BreakthroughExp { get; }
    }
    
    [OneRowConfig][Serializable]
    public class SettingData : ISettingData
    {
        public int LevelUpPrice;
        public int SkillUpPrice;
        public int StarUpPrice;
        public float BreakthroughMultiply;
        public int BreakthroughLevelPeriod;
        public int BreakthroughHeroesCount;
        public int BreakthroughEssence;
        public int BreakthroughExp;
        
        int ISettingData.LevelUpPrice => LevelUpPrice;
        int ISettingData.SkillUpPrice => SkillUpPrice;
        int ISettingData.StarUpPrice => StarUpPrice;
        float ISettingData.BreakthroughMultiply => BreakthroughMultiply;
        int ISettingData.BreakthroughLevelPeriod => BreakthroughLevelPeriod;
        int ISettingData.BreakthroughHeroesCount => BreakthroughHeroesCount;
        int ISettingData.BreakthroughEssence => BreakthroughEssence;
        int ISettingData.BreakthroughExp => BreakthroughExp;
    }
}