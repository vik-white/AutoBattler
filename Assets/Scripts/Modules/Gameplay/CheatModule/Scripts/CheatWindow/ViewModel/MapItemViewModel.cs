using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class MapItemViewModel: WindowViewModel<IMapData>
    {
        private readonly ILocationProvider _locationProvider;
        private readonly ISquadWindow _squadWindow;
        public string Title;
        public UnityAction OnSelect { get; set; }
        
        public MapItemViewModel(IMapData model, ILocationProvider locationProvider, ISquadWindow squadWindow) : base(model)
        {
            _locationProvider = locationProvider;
            _squadWindow = squadWindow;
            Title = model.ID;
            OnSelect = SelectLocation;
        }

        private void SelectLocation()
        {
            _locationProvider.ID = Model.ID;
            _squadWindow.ShowWindow();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}