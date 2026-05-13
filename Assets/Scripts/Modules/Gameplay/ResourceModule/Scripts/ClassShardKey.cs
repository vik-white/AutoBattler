using System;

namespace vikwhite
{
    public readonly struct ClassShardKey : IEquatable<ClassShardKey>
    {
        public readonly CharacterClassType Class;
        public readonly RarityType Rarity;

        public ClassShardKey(CharacterClassType @class, RarityType rarity)
        {
            Class = @class;
            Rarity = rarity;
        }

        public bool Equals(ClassShardKey other) => Class == other.Class && Rarity == other.Rarity;
        public override bool Equals(object obj) => obj is ClassShardKey other && Equals(other);
        public override int GetHashCode() => unchecked(((int)Class * 397) ^ (int)Rarity);
        public override string ToString() => $"{Class}_{Rarity}";

        public static bool operator ==(ClassShardKey a, ClassShardKey b) => a.Equals(b);
        public static bool operator !=(ClassShardKey a, ClassShardKey b) => !a.Equals(b);
    }
}
