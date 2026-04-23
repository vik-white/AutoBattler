using System.Collections.Generic;
using UnityEngine.Events;

namespace vikwhite
{
    public class LobbyWindowViewModel: WindowViewModel<bool>
    {
        public List<ResourceViewModel> Resources = new ();
        public UnityAction OnCheats;
        
        public LobbyWindowViewModel(bool model, ICheatWindow cheatWindow, IResourceService resource) : base(model)
        {
            OnCheats = cheatWindow.ShowWindow;
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Soft)));
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Hard)));
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnCheats = null;
        }
    }
}