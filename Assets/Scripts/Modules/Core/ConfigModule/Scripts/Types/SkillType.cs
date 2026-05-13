namespace vikwhite
{
    /// <summary>
    /// Describes how a skill behaves in battle. Independent from <see cref="SkillSlotType"/>
    /// which describes the role/slot the skill occupies on a character.
    /// </summary>
    public enum SkillType
    {
        None = -1,
        RangeAttack = 0,
        MeleeAttack = 1,
        OrbitAttack = 2,
        Buff = 3,
        Aura = 4,
        Abilities = 5,
        RearJump = 6,
    }
}
