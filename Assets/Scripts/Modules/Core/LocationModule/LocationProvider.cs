using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public interface ILocationProvider
    {
        string ID { get; set; }
        LocationType Type { get; set; }
        void SetLocation(string id);
    }
    
    public class LocationProvider : ILocationProvider
    {
        private readonly IConfigs _configs;
        public string ID { get; set; }
        public LocationType Type { get; set; }

        public LocationProvider(IConfigs configs)
        {
            _configs = configs;
        }

        public void SetLocation(string id)
        {
            if (id == null) id = "";
            foreach (var locationData in _configs.Map.GetAll())
            {
                if ((id == "" && locationData.RoadMap) || (id != "" && locationData.LocationID == id))
                {
                    ID = locationData.LocationID;
                    Type = locationData.LocationType;
                    return;
                }
            }
        }
    }
}