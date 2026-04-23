using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class DefeatWindowViewModel: WindowViewModel
    {
        public UnityAction OnEnd;
        
        public DefeatWindowViewModel(IEnvironmentStateMachine stateMachine)
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
