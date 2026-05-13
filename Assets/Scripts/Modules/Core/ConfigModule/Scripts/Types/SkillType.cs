namespace vikwhite
{
    public enum SkillType
    {
        None = -1,

        // Behaviors — describe how a skill acts in battle.
        RangeAttack = 0,
        MeleeAttack = 1,
        OrbitAttack = 2,
        Buff = 3,
        Aura = 4,
        Abilities = 5,
        RearJump = 6,

        // Slots — identify the role a skill plays for a character.
        Attack = 100,
        Active = 101,
        Passive1 = 102,
        Passive2 = 103,
        Meta1 = 104,
        Meta2 = 105,
        Meta3 = 106,
    }

    public static class SkillTypeExtensions
    {
        public static readonly SkillType[] CharacterSlots =
        {
            SkillType.Attack,
            SkillType.Active,
            SkillType.Passive1,
            SkillType.Passive2,
            SkillType.Meta1,
            SkillType.Meta2,
            SkillType.Meta3,
        };

        public static readonly SkillType[] UpgradableSlots =
        {
            SkillType.Active,
            SkillType.Passive1,
            SkillType.Passive2,
            SkillType.Meta1,
            SkillType.Meta2,
            SkillType.Meta3,
        };

        public static bool IsSlot(this SkillType type) => type >= SkillType.Attack;
    }
}
