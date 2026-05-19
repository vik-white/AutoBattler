using System;
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
        bool HasNextLocation { get; }
        void Initialize();
        void InitializePoints();
        Vector3 GetCurrentLocationPosition();
        void SetCurrentLocation(string id);
        void CompleteCurrentLocation();
    }

    public class SectorService : ISectorService
    {
        private readonly IProfileService _profile;
        private readonly IConfigs _configs;
        private readonly IEventDispatcher _dispatcher;
        private readonly List<string> _locationIDs = new();
        private readonly Dictionary<string, SectorPoint> _points = new();
        private string _currentLocation;

        public string CurrentLocation => _currentLocation;
        public string CurrentSector => _configs.Map.Get(_currentLocation).Sector;
        public bool HasNextLocation => !string.IsNullOrEmpty(GetNextLocationID(_currentLocation));

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
            _locationIDs.Clear();
            _locationIDs.AddRange(GetSectorLocationIDs(CurrentSector));
            _points.Clear();
            var points = UnityEngine.Object.FindObjectsByType<SectorPoint>(FindObjectsInactive.Include).OrderBy(point => point.Index);
            foreach (var point in points)
            {
                if (point.Index >= 0 && point.Index < _locationIDs.Count)
                {
                    var locationID = _locationIDs[point.Index];
                    point.Initialize(locationID);
                    _points[locationID] = point;
                }
            }
        }

        public void SetCurrentLocation(string id)
        {
            _currentLocation = id;
            _dispatcher.Dispatch(new SetSectorLocationEvent(_currentLocation));
        }

        public void CompleteCurrentLocation() => SetCurrentLocation(GetNextLocationID(_currentLocation));

        private string GetNextLocationID(string locationID)
        {
            var index = _locationIDs.IndexOf(locationID);
            if (index < 0 || index >= _locationIDs.Count - 1) return string.Empty;
            return _locationIDs[index + 1];
        }
        
        public Vector3 GetCurrentLocationPosition() => _points[_currentLocation].Position;

        private IReadOnlyList<string> GetSectorLocationIDs(string sector) => GetSectorLocations().Where(locationData => locationData.Sector == sector).Select(locationData => locationData.ID).ToList();

        private IEnumerable<IMapData> GetSectorLocations() => _configs.Map.GetAll().Where(locationData => !string.IsNullOrEmpty(locationData.Sector));
    }
}
