using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class LobbyWindowViewModel: WindowViewModel<bool>
    {
        private readonly ILocationProvider _locationProvider;
        private readonly ISquadWindow _squadWindow;
        private readonly IProfileService _profile;
        public List<ResourceViewModel> Resources = new ();
        public UnityAction OnCheats;
        public UnityAction OnFight;
        
        public LobbyWindowViewModel(bool model, ICheatWindow cheatWindow, IResourceService resource, ILocationProvider locationProvider, ISquadWindow squadWindow, IProfileService profile) : base(model)
        {
            _locationProvider = locationProvider;
            _squadWindow = squadWindow;
            _profile = profile;
            OnCheats = cheatWindow.ShowWindow;
            OnFight = SelectLocation;
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Soft)));
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Hard)));
        }
        
        private void SelectLocation()
        {
            _locationProvider.SetLocation(_profile.Data.RoadMapLocation);
            _squadWindow.ShowWindow();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnCheats = null;
            OnFight = null;
        }
    }
}