using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class VictoryWindowViewModel: WindowViewModel<bool>
    {
        public UnityAction OnEnd;
        public int Reward;
        
        public VictoryWindowViewModel(bool model, IEnvironmentStateMachine stateMachine, IResourceService resource, IConfigs configs, ILocationProvider location) : base(model)
        {
            Reward = configs.LocationStatic.Get(location.ID).Rewards;
            resource.Add(ResourceType.Soft, Reward);
            OnEnd = () => stateMachine.SwitchState(EnvironmentType.Lobby);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnEnd = null;
        }
    }
}