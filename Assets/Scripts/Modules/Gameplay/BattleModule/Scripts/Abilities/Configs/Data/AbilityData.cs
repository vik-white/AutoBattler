using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    public enum AbilityID
    {
        None = -1,
        RangeAttack = 0,
    }

    [Serializable]
    public struct AbilityData
    {
        public AbilityID ID;
        public int Level;
    }
}