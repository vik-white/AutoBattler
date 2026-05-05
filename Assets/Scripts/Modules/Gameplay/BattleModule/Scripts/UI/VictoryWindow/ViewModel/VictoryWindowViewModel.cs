using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class VictoryWindowViewModel: WindowViewModel
    {
        public UnityAction OnEnd;
        public List<RewardItemViewModel> Rewards = new();
        
        public VictoryWindowViewModel(
            IEnvironmentStateMachine stateMachine,
            IRewardFactory rewardFactory,
            IRewardService rewardService,
            IConfigs configs,
            ILocationProvider location,
            IRoadMapService roadMap)
        {
            var rewardId = configs.LocationStatic.Get(location.ID)?.Reward;
            var rewards = rewardFactory.Create(rewardId);

            rewardService.Add(rewards);
            
            foreach (var reward in rewards)
                Rewards.Add(CreateViewModel<RewardItemViewModel, Reward>(reward));

            roadMap.CompleteCurrentLocation();
            OnEnd = () => stateMachine.SwitchState(EnvironmentType.Lobby);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnEnd = null;
        }
    }
}
