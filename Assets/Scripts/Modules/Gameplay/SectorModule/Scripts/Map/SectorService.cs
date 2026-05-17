using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public interface ISectorService
    {
        void Initialize(string sectorID);
    }

    public class SectorService : ISectorService
    {
        private readonly IConfigs _configs;

        public SectorService(IConfigs configs)
        {
            _configs = configs;
        }

        public void Initialize(string sectorID)
        {
            var locationIDs = GetSectorLocationIDs(sectorID).ToList();
            var points = Object.FindObjectsByType<SectorPoint>(FindObjectsInactive.Include).OrderBy(point => point.Index);
            foreach (var point in points) point.Initialize(locationIDs[point.Index]);
        }
        
        public IReadOnlyList<string> GetSectorLocationIDs(string sector) => 
            _configs.Map.GetAll().Where(locationData => locationData.Sector == sector).Select(locationData => locationData.ID).ToList();
    }
}
