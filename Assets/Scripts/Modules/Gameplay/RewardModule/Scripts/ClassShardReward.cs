namespace vikwhite
{
    public class ClassShardReward : Reward
    {
        public CharacterClassType Class;
        public RarityType Rarity;

        public override bool IsSameAs(Reward other)
        {
            return other is ClassShardReward shard && shard.Class == Class && shard.Rarity == Rarity;
        }
    }
}
