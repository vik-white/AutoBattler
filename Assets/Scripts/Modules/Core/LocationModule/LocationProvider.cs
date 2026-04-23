using vikwhite.Data;

namespace vikwhite
{
    public interface ILocationProvider
    {
        string ID { get; set; }
        LocationType Type { get; set; }
        void SetNextRoadMapLocation();
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

        public void SetNextRoadMapLocation()
        {
            foreach (var locationData in _configs.Map.GetAll())
            {
                if (locationData.RoadMap)
                {
                    ID = locationData.LocationID;
                    Type = locationData.LocationType;
                    return;
                }
            }
        }
    }
}