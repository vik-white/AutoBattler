namespace vikwhite
{
    public class ShardReward : Reward
    {
        public string ID;

        public override bool IsSameAs(Reward other)
        {
            return other is ShardReward shard && shard.ID == ID;
        }
    }
}
