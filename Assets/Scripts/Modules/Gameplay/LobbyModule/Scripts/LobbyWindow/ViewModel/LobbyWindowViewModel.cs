using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class LobbyWindowViewModel: WindowViewModel<bool>
    {
        private readonly ICheatWindow _cheatWindow;
        public UnityAction OnCheats;
        
        public LobbyWindowViewModel(bool model, ICheatWindow cheatWindow) : base(model)
        {
            _cheatWindow = cheatWindow;
            OnCheats = _cheatWindow.ShowWindow;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnCheats = null;
        }
    }
}