using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class VictoryWindowViewModel: WindowViewModel
    {
        public UnityAction OnEnd;
        public int Reward;
        
        public VictoryWindowViewModel(IEnvironmentStateMachine stateMachine, IResourceService resource, IConfigs configs, ILocationProvider location, IRoadMapService roadMap)
        {
            Reward = 50;
            resource.Add(ResourceType.Soft, Reward);
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
