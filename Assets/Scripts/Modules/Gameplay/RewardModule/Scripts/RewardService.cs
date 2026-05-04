using System.Collections.Generic;

namespace vikwhite
{
    public interface IRewardService
    {
        void Add(Reward reward);
        void Add(IEnumerable<Reward> rewards);
    }

    public class RewardService : IRewardService
    {
        private readonly IResourceService _resources;
        private readonly IEventDispatcher _dispatcher;

        public RewardService(IResourceService resources, IEventDispatcher dispatcher)
        {
            _resources = resources;
            _dispatcher = dispatcher;
        }

        public void Add(IEnumerable<Reward> rewards)
        {
            if (rewards == null) return;
            foreach (var reward in rewards) Add(reward);
        }

        public void Add(Reward reward)
        {
            if (reward == null || reward.Value <= 0) return;

            switch (reward)
            {
                case ResourceReward res:
                    _resources.Add(res.ResourceType, res.Value);
                    break;
                case ShardReward shard:
                    _dispatcher.Dispatch(new AddShardEvent(shard.ID, shard.Value));
                    break;
            }
        }
    }
}
