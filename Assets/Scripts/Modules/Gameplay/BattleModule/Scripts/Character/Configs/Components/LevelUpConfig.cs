using vikwhite.Data;

namespace vikwhite.ECS
{
    public struct LevelUpConfig : IID
    {
        public uint ID { get; set; }
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
    }
}
