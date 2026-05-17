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
        public string CurrentLocation => _sector.CurrentLocation;
        public bool CanGoToNext => _sector.HasNextLocation && !_sector.IsMoving;
        public bool CanFight => !_sector.IsMoving;
        public UnityAction OnFight;
        public UnityAction OnLobby;
        public UnityAction OnGoToNext;
        public event Action Changed;

        public SectorWindowViewModel(ILocationProvider locationProvider, ISquadWindow squadWindow, ISectorService sector, IEnvironmentStateMachine environmentStateMachine)
        {
            _environmentStateMachine = environmentStateMachine;
            _locationProvider = locationProvider;
            _squadWindow = squadWindow;
            _sector = sector;
            OnFight = StartCurrentLocation;
            OnLobby = OpenLobby;
            OnGoToNext = GoToNext;
            _sector.Changed += OnSectorChanged;
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
            _sector.MoveToNextLocation();
            Changed?.Invoke();
        }

        private void OnSectorChanged()
        {
            Changed?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();
            _sector.Changed -= OnSectorChanged;
            OnFight = null;
            OnLobby = null;
            OnGoToNext = null;
            Changed = null;
        }
    }
}
