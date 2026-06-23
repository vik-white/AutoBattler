using vikwhite.Data;

namespace vikwhite
{
    public class SkillItemModel
    {
        public SkillSlotType Slot { get; }
        public ISkillData Skill { get; }

        public bool HasSkill => Skill != null;

        public SkillItemModel(SkillSlotType slot, ISkillData skill)
        {
            Slot = slot;
            Skill = skill;
        }
    }
}
