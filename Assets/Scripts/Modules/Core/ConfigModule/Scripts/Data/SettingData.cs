using System;

namespace vikwhite
{
    public interface ISettingData
    {
        int LevelUpPrice { get; }
        int SkillUpPrice { get; }
    }
    
    [OneRowConfig][Serializable]
    public class SettingData : ISettingData
    {
        public int LevelUpPrice;
        public int SkillUpPrice;
        
        int ISettingData.LevelUpPrice => LevelUpPrice;
        int ISettingData.SkillUpPrice => SkillUpPrice;
    }
}