using vikwhite.Data;

namespace vikwhite
{
    public class SkillItemModel
    {
        public SkillSlotType Slot { get; }
        public uint SkillID { get; }
        public ISkillData Skill { get; }
        public string Name { get; }

        public bool HasSkill => SkillID != 0 && Skill != null;

        public SkillItemModel(SkillSlotType slot, uint skillID, ISkillData skill, string name)
        {
            Slot = slot;
            SkillID = skillID;
            Skill = skill;
            Name = name;
        }
    }
}
