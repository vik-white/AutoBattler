using System.Collections.Generic;
using UniRx;
using UnityEngine.Events;

namespace vikwhite
{
    public class QuestItemViewModel : WindowViewModel<Quest>
    {
        private readonly IRewardService _rewardService;

        public string Description;
        public int Amount;
        public IReadOnlyReactiveProperty<int> Progress => Model.Progress;
        public IReadOnlyReactiveProperty<bool> Claimed => Model.Claimed;
        public IReadOnlyReactiveProperty<bool> Claimable;
        public List<RewardItemViewModel> Rewards = new();
        public UnityAction OnClaim;

        public QuestItemViewModel(Quest model, IRewardService rewardService) : base(model)
        {
            _rewardService = rewardService;
            Description = model.Description;
            Amount = model.Amount;

            Claimable = Model.Progress
                .CombineLatest(Model.Claimed, (progress, claimed) => progress >= Amount && claimed == false)
                .ToReactiveProperty();

            foreach (var reward in model.Rewards)
                Rewards.Add(CreateViewModel<RewardItemViewModel, Reward>(reward));

            OnClaim = Claim;
        }

        private void Claim()
        {
            if (Model.IsClaimable == false) return;
            _rewardService.Add(Model.Rewards);
            Model.Claimed.Value = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnClaim = null;
        }
    }
}
