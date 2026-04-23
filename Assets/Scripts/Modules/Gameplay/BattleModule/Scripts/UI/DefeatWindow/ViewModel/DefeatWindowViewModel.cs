using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class DefeatWindowViewModel: WindowViewModel<bool>
    {
        public UnityAction OnEnd;
        
        public DefeatWindowViewModel(bool model, IEnvironmentStateMachine stateMachine) : base(model)
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