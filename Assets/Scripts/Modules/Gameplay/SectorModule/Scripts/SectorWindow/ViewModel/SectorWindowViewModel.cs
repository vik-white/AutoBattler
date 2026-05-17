using UnityEngine.Events;

namespace vikwhite
{
    public class SectorWindowViewModel : WindowViewModel
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly ILocationProvider _locationProvider;
        private readonly ISquadWindow _squadWindow;
        private readonly ISectorService _sector;
        public string CurrentLocation;
        public UnityAction OnFight;
        public UnityAction OnLobby;

        public SectorWindowViewModel(ILocationProvider locationProvider, ISquadWindow squadWindow, ISectorService sector, IEnvironmentStateMachine environmentStateMachine)
        {
            _environmentStateMachine = environmentStateMachine;
            _locationProvider = locationProvider;
            _squadWindow = squadWindow;
            _sector = sector;
            OnFight = StartCurrentLocation;
            OnLobby = OpenLobby;
            CurrentLocation = sector.CurrentLocation;
        }

        private void StartCurrentLocation()
        {
            _sector.SetCurrentLocation(_sector.CurrentLocation);
            _locationProvider.ID = _sector.CurrentLocation;
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
