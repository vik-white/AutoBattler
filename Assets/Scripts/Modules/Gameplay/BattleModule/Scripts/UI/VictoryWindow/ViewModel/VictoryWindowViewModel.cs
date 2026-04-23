using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class VictoryWindowViewModel: WindowViewModel<bool>
    {
        public UnityAction OnEnd;
        
        public VictoryWindowViewModel(bool model, IConfigs configs) : base(model)
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