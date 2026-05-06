using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public class RewardsWindowViewModel : WindowViewModel<string>
    {
        public List<RewardItemViewModel> Rewards = new();

        public RewardsWindowViewModel(string rewardId, IRewardFactory rewardFactory, IRewardService rewardService) : base(rewardId)
        {
            var rewards = rewardFactory.Create(rewardId);
            rewardService.Add(rewards);

            foreach (var reward in rewards)
                Rewards.Add(CreateViewModel<RewardItemViewModel, Reward>(reward));
        }
    }
}
