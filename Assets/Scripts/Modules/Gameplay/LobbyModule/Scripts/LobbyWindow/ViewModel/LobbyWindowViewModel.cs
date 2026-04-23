using System.Collections.Generic;
using UnityEngine.Events;

namespace vikwhite
{
    public class LobbyWindowViewModel: WindowViewModel<bool>
    {
        public List<ResourceViewModel> Resources = new ();
        public UnityAction OnCheats;
        public UnityAction OnSquad;
        public UnityAction OnAddGold;
        public UnityAction OnRemoveGold;
        
        public LobbyWindowViewModel(bool model, ICheatWindow cheatWindow, ISquadWindow squadWindow, IResourceService resource) : base(model)
        {
            OnCheats = cheatWindow.ShowWindow;
            OnSquad = squadWindow.ShowWindow;
            OnAddGold = () => resource.Add(ResourceType.Soft, 100);
            OnRemoveGold = () => resource.Spend(ResourceType.Soft, 100);
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Soft)));
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Hard)));
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnCheats = null;
            OnSquad = null;
            OnAddGold = null;
            OnRemoveGold = null;
        }
    }
}