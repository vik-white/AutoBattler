using UnityEngine.Events;

namespace vikwhite
{
    public class SectorWindowViewModel : WindowViewModel
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly ILocationProvider _locationProvider;
        private readonly ISquadWindow _squadWindow;
        private readonly IRoadMapService _roadMap;
        public string CurrentLocation;
        public UnityAction OnFight;
        public UnityAction OnLobby;

        public SectorWindowViewModel(ILocationProvider locationProvider, ISquadWindow squadWindow, IRoadMapService roadMap, IEnvironmentStateMachine environmentStateMachine)
        {
            _environmentStateMachine = environmentStateMachine;
            _locationProvider = locationProvider;
            _squadWindow = squadWindow;
            _roadMap = roadMap;
            OnFight = StartCurrentLocation;
            OnLobby = OpenLobby;
            CurrentLocation = roadMap.CurrentLocation;
        }

        private void StartCurrentLocation()
        {
            _roadMap.SetCurrentLocation(_roadMap.CurrentLocation);
            _locationProvider.ID = _roadMap.CurrentLocation;
            _squadWindow.ShowWindow();
        }
        
        public void OpenLobby()
        {
            _environmentStateMachine.SwitchState(EnvironmentType.Lobby);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnFight = null;
            OnLobby = null;
        }
    }
}
