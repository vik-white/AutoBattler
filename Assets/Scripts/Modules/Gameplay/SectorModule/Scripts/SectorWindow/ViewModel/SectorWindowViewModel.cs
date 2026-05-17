using System;
using UnityEngine.Events;

namespace vikwhite
{
    public class SectorWindowViewModel : WindowViewModel
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly ILocationProvider _locationProvider;
        private readonly ISquadWindow _squadWindow;
        private readonly ISectorService _sector;
        private readonly ISectorStartState _sectorStartState;
        public string CurrentLocation => _sector.CurrentLocation;
        public bool CanGoToNext => _sector.HasNextLocation && !_sectorStartState.IsMoving;
        public bool CanFight => !_sectorStartState.IsMoving;
        public UnityAction OnFight;
        public UnityAction OnLobby;
        public UnityAction OnGoToNext;
        public event Action Changed;

        public SectorWindowViewModel(ILocationProvider locationProvider, ISquadWindow squadWindow, ISectorService sector, ISectorStartState sectorStartState, IEnvironmentStateMachine environmentStateMachine)
        {
            _environmentStateMachine = environmentStateMachine;
            _locationProvider = locationProvider;
            _squadWindow = squadWindow;
            _sector = sector;
            _sectorStartState = sectorStartState;
            OnFight = StartCurrentLocation;
            OnLobby = OpenLobby;
            OnGoToNext = GoToNext;
            _sectorStartState.Changed += OnSectorChanged;
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

        private void GoToNext()
        {
            _sectorStartState.MoveToNextLocation();
        }

        private void OnSectorChanged()
        {
            Changed?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();
            _sectorStartState.Changed -= OnSectorChanged;
            OnFight = null;
            OnLobby = null;
            OnGoToNext = null;
            Changed = null;
        }
    }
}
