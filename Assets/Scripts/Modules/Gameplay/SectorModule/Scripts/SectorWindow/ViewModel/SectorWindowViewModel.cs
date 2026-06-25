using System;
using UnityEngine;
using UnityEngine.Events;

namespace vikwhite
{
    public class SectorWindowViewModel : WindowViewModel<SectorPlayer>
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly ILocationProvider _locationProvider;
        private readonly ISectorService _sector;
        private readonly SectorPlayer _player;
        
        public int CurrentLocationIndex => _sector.CurrentLocationIndex;
        public bool CanFight => !_player.IsMoving;
        public UnityAction OnFight;
        public UnityAction OnLobby;
        public UnityAction OnGoToNext;

        public SectorWindowViewModel(SectorPlayer sectorPlayer, ILocationProvider locationProvider, ISectorService sector, IEnvironmentStateMachine environmentStateMachine): base(sectorPlayer)
        {
            _environmentStateMachine = environmentStateMachine;
            _locationProvider = locationProvider;
            _sector = sector;
            _player = sectorPlayer;
            OnFight = StartCurrentLocation;
            OnLobby = OpenLobby;
            OnGoToNext = GoToNext;
        }

        private void StartCurrentLocation()
        {
            _sector.SetCurrentLocation(_sector.CurrentLocation);
            _locationProvider.ID = _sector.CurrentLocation;
            _environmentStateMachine.SwitchState(EnvironmentType.Battle);
        }
        
        public void OpenLobby() => _environmentStateMachine.SwitchState(EnvironmentType.Lobby);

        private void GoToNext()
        {
            _sector.CompleteCurrentLocation();
            _player.Move(_sector.GetCurrentLocationPath());
        }

        public override void Dispose()
        {
            base.Dispose();
            OnFight = null;
            OnLobby = null;
            OnGoToNext = null;
        }
    }
}
