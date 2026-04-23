using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class DefeatWindowViewModel: WindowViewModel<bool>
    {
        public UnityAction OnEnd;
        
        public DefeatWindowViewModel(bool model, IConfigs configs) : base(model)
        {
            OnEnd = EndBattle;
        }

        private void EndBattle()
        {
            Close();
        }

        public override void Dispose()
        {
            base.Dispose();
            OnEnd = null;
        }
    }
}