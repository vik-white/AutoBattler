using UnityEngine.Events;

namespace vikwhite
{
    public class MapItemViewModel: WindowViewModel<string>
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly ILocationProvider _locationProvider;
        private readonly string _locationID;
        public string Title;
        public UnityAction OnSelect { get; set; }
        
        public MapItemViewModel(string model, IEnvironmentStateMachine environmentStateMachine, ILocationProvider locationProvider) : base(model)
        {
            _environmentStateMachine = environmentStateMachine;
            _locationProvider = locationProvider;
            _locationID = model;
            Title = model;
            OnSelect = StartLocation;
        }

        private void StartLocation()
        {
            _locationProvider.Location = _locationID;
            _environmentStateMachine.SwitchState(EnvironmentType.Battle);

        }
    }
}