using System;

namespace vikwhite
{
    public interface IStarLockSkillData
    {
        int ID { get; }
        int SkillActive { get; }
        int SkillPassive1 { get; }
        int SkillPassive2 { get; }
        int SkillMeta1 { get; }
        int SkillMeta2 { get; }
        int SkillMeta3 { get; }
    }

    [Serializable]
    public class StarLockSkillSkillData : IStarLockSkillData
    {
        public int ID;
        public int SkillActive;
        public int SkillPassive1;
        public int SkillPassive2;
        public int SkillMeta1;
        public int SkillMeta2;
        public int SkillMeta3;
        
        int IStarLockSkillData.ID => ID;
        int IStarLockSkillData.SkillActive => SkillActive;
        int IStarLockSkillData.SkillPassive1 => SkillPassive1;
        int IStarLockSkillData.SkillPassive2 => SkillPassive2;
        int IStarLockSkillData.SkillMeta1 => SkillMeta1;
        int IStarLockSkillData.SkillMeta2 => SkillMeta2;
        int IStarLockSkillData.SkillMeta3 => SkillMeta3;
    }
}