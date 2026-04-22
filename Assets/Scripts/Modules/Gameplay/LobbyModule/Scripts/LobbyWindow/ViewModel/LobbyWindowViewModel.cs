using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class LobbyWindowViewModel: WindowViewModel<bool>
    {
        public UnityAction OnCheats;
        public UnityAction OnSquad;
        
        public LobbyWindowViewModel(bool model, ICheatWindow cheatWindow, ISquadWindow squadWindow) : base(model)
        {
            OnCheats = cheatWindow.ShowWindow;
            OnSquad = squadWindow.ShowWindow;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnCheats = null;
            OnSquad = null;
        }
    }
}