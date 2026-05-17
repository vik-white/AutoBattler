using System.Collections.Generic;
using System.Linq;
using vikwhite.Data;

namespace vikwhite
{
    public interface IRoadMapService
    {
        string CurrentLocation { get; }
        void Initialize();
        IReadOnlyList<string> GetSectorLocationIDs(string sector);
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

        public RoadMapService(IProfileService profile, IConfigs configs, IEventDispatcher dispatcher)
        {
            _profile = profile;
            _configs = configs;
            _dispatcher = dispatcher;
        }

        public void Initialize()
        {
            _currentLocation = _profile.Data.RoadMapLocation;
            if (!IsValidRoadMapLocation(_currentLocation))
            {
                var firstRoadMapLocation = GetRoadMapLocations().FirstOrDefault();
                if (firstRoadMapLocation != null)
                    SetCurrentLocation(firstRoadMapLocation.ID);
            }
        }

        public IReadOnlyList<string> GetSectorLocationIDs(string sector)
        {
            if (string.IsNullOrEmpty(sector)) return new List<string>();

            return _configs.Map.GetAll()
                .Where(locationData => locationData.Sector == sector)
                .Select(locationData => locationData.ID)
                .ToList();
        }

        public void SetCurrentLocation(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!_configs.Map.Contains(id)) return;

            _currentLocation = id;
            _dispatcher.Dispatch(new SetRoadMapLocationEvent(_currentLocation));
        }

        public void CompleteCurrentLocation()
        {
            string nextLocation = _currentLocation;
            bool currentLocationFound = false;
            var currentLocationData = _configs.Map.Get(_currentLocation);
            var roadMapLocations = currentLocationData != null
                ? GetRoadMapLocations().Where(locationData => locationData.Sector == currentLocationData.Sector)
                : GetRoadMapLocations();

            foreach (var locationData in roadMapLocations)
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

        private IEnumerable<IMapData> GetRoadMapLocations()
        {
            return _configs.Map.GetAll()
                .Where(locationData => locationData.RoadMap);
        }

        private bool IsValidRoadMapLocation(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            var locationData = _configs.Map.Get(id);
            return locationData != null && locationData.RoadMap;
        }
    }
}
