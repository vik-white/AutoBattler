using System;

namespace vikwhite
{
    public interface IStarData
    {
        int ID { get; }
        int Level { get; }
        int SkillActive { get; }
        int SkillPassive1 { get; }
        int SkillPassive2 { get; }
        int SkillMeta1 { get; }
        int SkillMeta2 { get; }
        int SkillMeta3 { get; }
    }

    [Serializable]
    public class StarData : IStarData
    {
        public int ID;
        public int Level;
        public int SkillActive;
        public int SkillPassive1;
        public int SkillPassive2;
        public int SkillMeta1;
        public int SkillMeta2;
        public int SkillMeta3;
        
        int IStarData.ID => ID;
        int IStarData.Level => Level;
        int IStarData.SkillActive => SkillActive;
        int IStarData.SkillPassive1 => SkillPassive1;
        int IStarData.SkillPassive2 => SkillPassive2;
        int IStarData.SkillMeta1 => SkillMeta1;
        int IStarData.SkillMeta2 => SkillMeta2;
        int IStarData.SkillMeta3 => SkillMeta3;
    }
}