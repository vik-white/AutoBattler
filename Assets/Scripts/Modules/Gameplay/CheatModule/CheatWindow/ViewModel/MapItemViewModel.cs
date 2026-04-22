using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class MapItemViewModel: WindowViewModel<IMapData>
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly ILocationProvider _locationProvider;
        public string Title;
        public UnityAction OnSelect { get; set; }
        
        public MapItemViewModel(IMapData model, IEnvironmentStateMachine environmentStateMachine, ILocationProvider locationProvider) : base(model)
        {
            _environmentStateMachine = environmentStateMachine;
            _locationProvider = locationProvider;
            Title = model.LocationID;
            OnSelect = StartLocation;
        }

        private void StartLocation()
        {
            _locationProvider.ID = Model.LocationID;
            _locationProvider.Type = Model.LocationType;
            _environmentStateMachine.SwitchState(EnvironmentType.Battle);
        }
    }
}