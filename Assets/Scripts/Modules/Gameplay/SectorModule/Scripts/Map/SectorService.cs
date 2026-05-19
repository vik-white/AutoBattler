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
        event Action Changed;
        void Initialize();
        void InitializePoints();
        Vector3 GetCurrentLocationPosition();
        bool TryGetNextLocation(out string locationID, out Vector3 position);
        void SetCurrentLocation(string id);
        void CompleteCurrentLocation();
    }

    public class SectorService : ISectorService
    {
        private readonly IProfileService _profile;
        private readonly IConfigs _configs;
        private readonly IEventDispatcher _dispatcher;
        private readonly List<string> _sectorLocationIDs = new();
        private readonly Dictionary<string, SectorPoint> _pointsByLocation = new();
        private string _currentLocation;

        public string CurrentLocation => _currentLocation;
        public string CurrentSector => GetLocationSector(_currentLocation);
        public bool HasNextLocation => !string.IsNullOrEmpty(GetNextLocationID(_currentLocation));
        public event Action Changed;

        public SectorService(IProfileService profile, IConfigs configs, IEventDispatcher dispatcher)
        {
            _profile = profile;
            _configs = configs;
            _dispatcher = dispatcher;
        }

        public void Initialize()
        {
            _currentLocation = _profile.Data.RoadMapLocation;
            if (IsValidLocation(_currentLocation)) return;

            var firstLocation = GetSectorLocations().FirstOrDefault();
            if (firstLocation != null) SetCurrentLocation(firstLocation.ID);
        }

        public void InitializePoints()
        {
            _sectorLocationIDs.Clear();
            _sectorLocationIDs.AddRange(GetSectorLocationIDs(CurrentSector));

            _pointsByLocation.Clear();
            var points = UnityEngine.Object.FindObjectsByType<SectorPoint>(FindObjectsInactive.Include).OrderBy(point => point.Index);
            foreach (var point in points)
            {
                if (point.Index >= 0 && point.Index < _sectorLocationIDs.Count)
                {
                    var locationID = _sectorLocationIDs[point.Index];
                    point.Initialize(locationID, locationID);
                    _pointsByLocation[locationID] = point;
                }
                else
                {
                    point.Clear();
                }
            }

            Changed?.Invoke();
        }

        private IReadOnlyList<string> GetSectorLocationIDs(string sector)
        {
            if (string.IsNullOrEmpty(sector)) return new List<string>();

            return GetSectorLocations()
                .Where(locationData => locationData.Sector == sector)
                .Select(locationData => locationData.ID)
                .ToList();
        }

        public Vector3 GetCurrentLocationPosition()
        {
            TryGetLocationPosition(_currentLocation, out var position);
            return position;
        }

        public bool TryGetNextLocation(out string locationID, out Vector3 position)
        {
            locationID = GetNextLocationID(_currentLocation);
            if (string.IsNullOrEmpty(locationID))
            {
                position = default;
                return false;
            }

            return TryGetLocationPosition(locationID, out position);
        }

        public void SetCurrentLocation(string id)
        {
            if (!IsValidLocation(id)) return;

            _currentLocation = id;
            _dispatcher.Dispatch(new SetSectorLocationEvent(_currentLocation));
            Changed?.Invoke();
        }

        public void CompleteCurrentLocation()
        {
            var nextLocation = GetNextLocationID(_currentLocation);
            if (!string.IsNullOrEmpty(nextLocation))
                SetCurrentLocation(nextLocation);
        }

        private bool TryGetLocationPosition(string locationID, out Vector3 position)
        {
            if (!string.IsNullOrEmpty(locationID) && _pointsByLocation.TryGetValue(locationID, out var point))
            {
                position = point.Position;
                return true;
            }

            position = default;
            return false;
        }

        private string GetNextLocationID(string locationID)
        {
            if (_sectorLocationIDs.Count == 0)
                _sectorLocationIDs.AddRange(GetSectorLocationIDs(CurrentSector));

            var index = _sectorLocationIDs.IndexOf(locationID);
            if (index < 0 || index >= _sectorLocationIDs.Count - 1) return string.Empty;

            return _sectorLocationIDs[index + 1];
        }

        private IEnumerable<IMapData> GetSectorLocations()
        {
            return _configs.Map.GetAll()
                .Where(locationData => !string.IsNullOrEmpty(locationData.Sector));
        }

        private bool IsValidLocation(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;

            var locationData = _configs.Map.Get(id);
            return locationData != null && !string.IsNullOrEmpty(locationData.Sector);
        }

        private string GetLocationSector(string id)
        {
            if (string.IsNullOrEmpty(id)) return string.Empty;

            var locationData = _configs.Map.Get(id);
            return locationData?.Sector ?? string.Empty;
        }
    }
}
