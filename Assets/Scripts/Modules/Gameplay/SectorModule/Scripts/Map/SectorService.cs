using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public interface ISectorService
    {
        string CurrentLocation { get; }
        string CurrentSector { get; }
        void Initialize();
        void InitializePoints();
        void SetCurrentLocation(string id);
        void CompleteCurrentLocation();
    }

    public class SectorService : ISectorService
    {
        private readonly IProfileService _profile;
        private readonly IConfigs _configs;
        private readonly IEventDispatcher _dispatcher;
        private string _currentLocation;

        public string CurrentLocation => _currentLocation;
        public string CurrentSector => _configs.Map.Get(_currentLocation).Sector;

        public SectorService(IProfileService profile, IConfigs configs, IEventDispatcher dispatcher)
        {
            _profile = profile;
            _configs = configs;
            _dispatcher = dispatcher;
        }

        public void Initialize()
        {
            _currentLocation = _profile.Data.RoadMapLocation;
        }

        public void InitializePoints()
        {
            var locationIDs = GetSectorLocationIDs(CurrentSector).ToList();
            var points = Object.FindObjectsByType<SectorPoint>(FindObjectsInactive.Include).OrderBy(point => point.Index);
            foreach (var point in points) point.Initialize(locationIDs[point.Index]);
        }

        private IReadOnlyList<string> GetSectorLocationIDs(string sector)
        {
            return _configs.Map.GetAll().Where(locationData => locationData.Sector == sector).Select(locationData => locationData.ID).ToList();
        }

        public void SetCurrentLocation(string id)
        {
            _currentLocation = id;
            _dispatcher.Dispatch(new SetSectorLocationEvent(_currentLocation));
        }

        public void CompleteCurrentLocation()
        {
            string nextLocation = _currentLocation;
            bool currentLocationFound = false;

            foreach (var locationData in _configs.Map.GetAll())
            {
                if (currentLocationFound)
                {
                    nextLocation = locationData.ID;
                    break;
                }

                if (locationData.ID == _currentLocation) currentLocationFound = true;
            }

            SetCurrentLocation(nextLocation);
        }
    }
}
