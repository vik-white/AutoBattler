using System.Collections.Generic;
using System.Linq;
using vikwhite.Data;

namespace vikwhite
{
    public interface IRoadMapService
    {
        string CurrentLocation { get; }
        string CurrentSector { get; }
        void Initialize();
        void SetCurrentLocation(string id);
        void CompleteCurrentLocation();
    }
    
    public class RoadMapService : IRoadMapService
    {
        private readonly IProfileService _profile;
        private readonly IConfigs _configs;
        private readonly IEventDispatcher _dispatcher;
        private string _currentLocation;
        public string CurrentLocation => _currentLocation;
        public string CurrentSector => _configs.Map.Get(_currentLocation).Sector;

        public RoadMapService(IProfileService profile, IConfigs configs, IEventDispatcher dispatcher)
        {
            _profile = profile;
            _configs = configs;
            _dispatcher = dispatcher;
        }

        public void Initialize()
        {
            _currentLocation = _profile.Data.RoadMapLocation;
        }

        public void SetCurrentLocation(string id)
        {
            _currentLocation = id;
            _dispatcher.Dispatch(new SetRoadMapLocationEvent(_currentLocation));
        }

        public void CompleteCurrentLocation()
        {
            string nextLocation = _currentLocation;
            bool currentLocationFound = false;
            foreach (var locationData in _configs.Map.GetAll().Where(locationData => locationData.RoadMap))
            {
                if (currentLocationFound)
                {
                    nextLocation = locationData.ID;
                    break;
                }
                if (locationData.ID == _currentLocation) currentLocationFound = true;
            }

            _currentLocation = nextLocation;
            _dispatcher.Dispatch(new SetRoadMapLocationEvent(_currentLocation));
        }
    }
}
