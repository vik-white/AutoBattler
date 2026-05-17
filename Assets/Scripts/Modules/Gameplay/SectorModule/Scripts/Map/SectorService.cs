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
        bool IsMoving { get; }
        event Action Changed;
        void Initialize();
        void SetPlayerModel(ISectorPlayerModel player);
        void ClearPlayerModel();
        void InitializePoints();
        void MoveToNextLocation();
        void SetCurrentLocation(string id);
        void CompleteCurrentLocation();
    }

    public class SectorService : ISectorService
    {
        private readonly IProfileService _profile;
        private readonly IConfigs _configs;
        private readonly IEventDispatcher _dispatcher;
        private ISectorPlayerModel _player;
        private readonly List<string> _sectorLocationIDs = new();
        private readonly Dictionary<string, SectorPoint> _pointsByLocation = new();
        private string _currentLocation;
        private string _movingLocation;

        public string CurrentLocation => _currentLocation;
        public string CurrentSector => GetLocationSector(_currentLocation);
        public bool HasNextLocation => !string.IsNullOrEmpty(GetNextLocationID(_currentLocation));
        public bool IsMoving => _player != null && _player.IsMoving;
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
            if (firstLocation != null)
                SetCurrentLocation(firstLocation.ID);
        }

        public void SetPlayerModel(ISectorPlayerModel player)
        {
            ClearPlayerModel();
            _player = player;
            if (_player != null) _player.MovementCompleted += OnPlayerMovementCompleted;
        }

        public void ClearPlayerModel()
        {
            if (_player != null)
                _player.MovementCompleted -= OnPlayerMovementCompleted;

            _player = null;
            _movingLocation = string.Empty;
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

            MovePlayerToCurrentLocation();
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

        public void MoveToNextLocation()
        {
            if (_player == null || _player.IsMoving) return;

            var nextLocation = GetNextLocationID(_currentLocation);
            if (string.IsNullOrEmpty(nextLocation)) return;
            if (!_pointsByLocation.TryGetValue(nextLocation, out var nextPoint)) return;

            _movingLocation = nextLocation;
            _player.MoveTo(nextPoint.Position);
            Changed?.Invoke();
        }

        public void SetCurrentLocation(string id)
        {
            if (!IsValidLocation(id)) return;

            _currentLocation = id;
            _dispatcher.Dispatch(new SetSectorLocationEvent(_currentLocation));
            MovePlayerToCurrentLocation();
            Changed?.Invoke();
        }

        public void CompleteCurrentLocation()
        {
            var nextLocation = GetNextLocationID(_currentLocation);
            if (!string.IsNullOrEmpty(nextLocation))
                SetCurrentLocation(nextLocation);
        }

        private void MovePlayerToCurrentLocation()
        {
            if (_player == null) return;

            if (_pointsByLocation.TryGetValue(_currentLocation, out var currentPoint))
                _player.PlaceAt(currentPoint.Position);
        }

        private void OnPlayerMovementCompleted()
        {
            if (string.IsNullOrEmpty(_movingLocation))
            {
                Changed?.Invoke();
                return;
            }

            var completedLocation = _movingLocation;
            _movingLocation = string.Empty;
            SetCurrentLocation(completedLocation);
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
