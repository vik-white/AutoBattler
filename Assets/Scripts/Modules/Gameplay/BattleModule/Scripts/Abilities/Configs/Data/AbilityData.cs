using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    public enum AbilityID
    {
        None = -1,
        Gun = 0,
        DamageMultiply = 1,
        CooldownMultiply = 2
    }

    [Serializable]
    public struct AbilityData
    {
        public AbilityID ID;
        public int Level;
    }
}