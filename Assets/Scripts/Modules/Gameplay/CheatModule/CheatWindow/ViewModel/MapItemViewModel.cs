using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class MapItemViewModel: WindowViewModel<IMapData>
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly ILocationProvider _locationProvider;
        private readonly ICheatWindow _cheatWindow;
        public string Title;
        public UnityAction OnSelect { get; set; }
        
        public MapItemViewModel(IMapData model, IEnvironmentStateMachine environmentStateMachine, ILocationProvider locationProvider, ICheatWindow cheatWindow) : base(model)
        {
            _environmentStateMachine = environmentStateMachine;
            _locationProvider = locationProvider;
            _cheatWindow = cheatWindow;
            Title = model.LocationID;
            OnSelect = StartLocation;
        }

        private void StartLocation()
        {
            _locationProvider.ID = Model.LocationID;
            _locationProvider.Type = Model.LocationType;
            _cheatWindow.CloseWindow();
            _environmentStateMachine.SwitchState(EnvironmentType.Battle);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}