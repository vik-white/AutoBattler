using System;

namespace vikwhite.ECS
{
    public enum AbilityID
    {
        None = -1,
        RangeAttack = 0,
        MeleeAttack = 1,
    }

    [Serializable]
    public struct AbilityData
    {
        public AbilityID ID;
        public int Level;
    }
}