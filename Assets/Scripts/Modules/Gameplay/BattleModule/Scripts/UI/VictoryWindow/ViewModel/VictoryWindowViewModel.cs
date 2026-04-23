using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class VictoryWindowViewModel: WindowViewModel<bool>
    {
        public UnityAction OnEnd;
        
        public VictoryWindowViewModel(bool model, IEnvironmentStateMachine stateMachine) : base(model)
        {
            OnEnd = () => stateMachine.SwitchState(EnvironmentType.Lobby);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnEnd = null;
        }
    }
}