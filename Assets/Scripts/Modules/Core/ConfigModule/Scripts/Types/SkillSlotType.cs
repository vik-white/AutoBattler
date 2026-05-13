namespace vikwhite
{
    /// <summary>
    /// Identifies a skill's role on a character (which slot it occupies).
    /// Independent from <see cref="SkillType"/> which describes the skill's behavior.
    /// </summary>
    public enum SkillSlotType
    {
        None = -1,
        Attack = 0,
        Active = 1,
        Passive1 = 2,
        Passive2 = 3,
        Meta1 = 4,
        Meta2 = 5,
        Meta3 = 6,
    }

    public static class SkillSlotExtensions
    {
        public static readonly SkillSlotType[] CharacterSlots =
        {
            SkillSlotType.Attack,
            SkillSlotType.Active,
            SkillSlotType.Passive1,
            SkillSlotType.Passive2,
            SkillSlotType.Meta1,
            SkillSlotType.Meta2,
            SkillSlotType.Meta3,
        };

        public static readonly SkillSlotType[] UpgradableSlots =
        {
            SkillSlotType.Active,
            SkillSlotType.Passive1,
            SkillSlotType.Passive2,
            SkillSlotType.Meta1,
            SkillSlotType.Meta2,
            SkillSlotType.Meta3,
        };
    }
}
