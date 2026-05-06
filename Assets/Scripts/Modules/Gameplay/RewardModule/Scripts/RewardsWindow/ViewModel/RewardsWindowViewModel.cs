using System.Collections.Generic;

namespace vikwhite
{
    public class RewardsWindowViewModel : WindowViewModel<List<Reward>>
    {
        public List<RewardItemViewModel> Rewards = new();

        public RewardsWindowViewModel(List<Reward> rewards, IRewardService rewardService) : base(rewards)
        {
            rewardService.Add(rewards);

            foreach (var reward in rewards)
                Rewards.Add(CreateViewModel<RewardItemViewModel, Reward>(reward));
        }
    }
}
