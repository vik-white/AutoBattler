using UniRx;

namespace vikwhite
{
    public class ClassShard
    {
        public ClassShardKey Key;
        public ReactiveProperty<int> Amount;

        public CharacterClassType Class => Key.Class;
        public RarityType Rarity => Key.Rarity;

        public ClassShard(ClassShardKey key, int amount)
        {
            Key = key;
            Amount = new ReactiveProperty<int>(amount);
        }

        public ClassShard(CharacterClassType @class, RarityType rarity, int amount)
            : this(new ClassShardKey(@class, rarity), amount) { }
    }
}
