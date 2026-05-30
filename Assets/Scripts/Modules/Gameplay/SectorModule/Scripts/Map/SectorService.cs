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
        void SetCurrentLocation(string id);
        void CompleteCurrentLocation();
        SectorPoint GetCurrentLocationPoint();
        BezierPath GetCurrentLocationPath();
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
            InitializeLocationIDs();
        }

        public void InitializePoints()
        {
            InitializeLocationIDs();
            _points.Clear();
            var points = UnityEngine.Object.FindObjectsByType<SectorPoint>(FindObjectsInactive.Include).OrderBy(point => point.Index);
            foreach (var point in points)
            {
                if (point.Index >= 0 && point.Index < _locationIDs.Count)
                {
                    var locationID = _locationIDs[point.Index];
                    point.Initialize(_configs.Map.Get(locationID), IsCharacterVisible(point.Index));
                    _points[locationID] = point;
                }
            }
        }

        public void SetCurrentLocation(string id)
        {
            _currentLocation = id;
            _dispatcher.Dispatch(new SetSectorLocationEvent(_currentLocation));
            UpdatePointCharactersVisibility();
        }

        public void CompleteCurrentLocation()
        {
            var nextLocationID = GetNextLocationID(_currentLocation);
            if (string.IsNullOrEmpty(nextLocationID)) return;
            SetCurrentLocation(nextLocationID);
        }

        public SectorPoint GetCurrentLocationPoint()
        {
            return _points.TryGetValue(_currentLocation, out var point) ? point : null;
        }

        public BezierPath GetCurrentLocationPath() => GetCurrentLocationPoint()?.Path;

        private void InitializeLocationIDs()
        {
            _locationIDs.Clear();
            _locationIDs.AddRange(GetSectorLocationIDs(CurrentSector));
        }

        private void UpdatePointCharactersVisibility()
        {
            foreach (var point in _points.Values)
            {
                point.SetCharacterVisible(IsCharacterVisible(point.Index));
            }
        }

        private bool IsCharacterVisible(int pointIndex)
        {
            var currentIndex = _locationIDs.IndexOf(_currentLocation);
            return currentIndex < 0 || pointIndex >= currentIndex;
        }

        private string GetNextLocationID(string locationID)
        {
            var index = _locationIDs.IndexOf(locationID);
            if (index < 0 || index >= _locationIDs.Count - 1) return string.Empty;
            return _locationIDs[index + 1];
        }
        private IReadOnlyList<string> GetSectorLocationIDs(string sector) => GetSectorLocations().Where(locationData => locationData.Sector == sector).Select(locationData => locationData.ID).ToList();

        private IEnumerable<IMapData> GetSectorLocations() => _configs.Map.GetAll().Where(locationData => !string.IsNullOrEmpty(locationData.Sector));
    }
}
