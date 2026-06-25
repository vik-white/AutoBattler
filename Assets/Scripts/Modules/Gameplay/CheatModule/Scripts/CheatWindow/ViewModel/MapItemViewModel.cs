using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class MapItemViewModel: WindowViewModel<IMapData>
    {
        private readonly ILocationProvider _locationProvider;
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        public string Title;
        public UnityAction OnSelect { get; set; }
        
        public MapItemViewModel(IMapData model, ILocationProvider locationProvider, IEnvironmentStateMachine environmentStateMachine) : base(model)
        {
            _locationProvider = locationProvider;
            _environmentStateMachine = environmentStateMachine;
            Title = model.ID;
            OnSelect = SelectLocation;
        }

        private void SelectLocation()
        {
            _locationProvider.ID = Model.ID;
            _environmentStateMachine.SwitchState(EnvironmentType.Battle);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}
