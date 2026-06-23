using vikwhite.Data;

namespace vikwhite
{
    public class SkillItemModel
    {
        public SkillSlotType Slot { get; }
        public ISkillData Skill { get; }
        public string Name { get; }

        public bool HasSkill => Skill != null;

        public SkillItemModel(SkillSlotType slot, ISkillData skill, string name)
        {
            Slot = slot;
            Skill = skill;
            Name = name;
        }
    }
}
