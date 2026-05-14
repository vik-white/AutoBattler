using System;

namespace vikwhite
{
    public interface ISettingData
    {
        int LevelUpPrice { get; }
        int SkillUpPrice { get; }
        int StarUpPrice { get; }
    }
    
    [OneRowConfig][Serializable]
    public class SettingData : ISettingData
    {
        public int LevelUpPrice;
        public int SkillUpPrice;
        public int StarUpPrice;
        
        int ISettingData.LevelUpPrice => LevelUpPrice;
        int ISettingData.SkillUpPrice => SkillUpPrice;
        int ISettingData.StarUpPrice => StarUpPrice;
    }
}