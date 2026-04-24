using vikwhite.Data;

namespace vikwhite
{
    public interface IRoadMapService
    {
        string CurrentLocation { get; }
        void Initialize();
    }
    
    public class RoadMapService : IRoadMapService
    {
        private readonly IProfileService _profile;
        private readonly IConfigs _configs;
        private readonly IEventDispatcher _dispatcher;
        private string _currentLocation;
        public string CurrentLocation => _currentLocation;

        public RoadMapService(IProfileService profile, IConfigs configs, IEventDispatcher dispatcher)
        {
            _profile = profile;
            _configs = configs;
            _dispatcher = dispatcher;
        }

        public void Initialize()
        {
            _currentLocation = _profile.Data.RoadMapLocation;
            if (_currentLocation == "" || _currentLocation == null)
            {
                foreach (var locationData in _configs.Map.GetAll())
                {
                    if(!locationData.RoadMap) continue;
                    _currentLocation = locationData.ID;
                    _dispatcher.Dispatch(new SetRoadMapLocationEvent(_currentLocation));
                    break;
                }
            }
        }
    }
}