using System.Linq;
using UnityEngine;

namespace vikwhite
{
    public interface ISectorMapService
    {
        void Initialize();
        void SelectLocation(SectorPoint point);
    }

    public class SectorMapService : ISectorMapService
    {
        private readonly IRoadMapService _roadMap;
        private readonly ILocationProvider _locationProvider;
        private readonly ISquadWindow _squadWindow;

        public SectorMapService(
            IRoadMapService roadMap,
            ILocationProvider locationProvider,
            ISquadWindow squadWindow)
        {
            _roadMap = roadMap;
            _locationProvider = locationProvider;
            _squadWindow = squadWindow;
        }

        public void Initialize()
        {
            var sectorMap = Object.FindAnyObjectByType<SectorMap>();
            if (sectorMap == null)
            {
                Debug.LogWarning("Sector map was not found on the loaded sector scene.");
                return;
            }

            var locationIDs = _roadMap.GetSectorLocationIDs(sectorMap.SectorID).ToList();
            var points = Object
                .FindObjectsByType<SectorPoint>(FindObjectsInactive.Include)
                .OrderBy(point => point.Index);

            foreach (var point in points)
            {
                point.Initialize(this);
                if (point.Index >= 0 && point.Index < locationIDs.Count)
                {
                    var locationID = locationIDs[point.Index];
                    point.SetLocation(locationID, locationID);
                }
                else
                {
                    point.ClearLocation();
                }
            }
        }

        public void SelectLocation(SectorPoint point)
        {
            if (point == null || !point.HasLocation) return;

            _roadMap.SetCurrentLocation(point.LocationID);
            _locationProvider.ID = point.LocationID;
            _squadWindow.ShowWindow();
        }
    }
}
