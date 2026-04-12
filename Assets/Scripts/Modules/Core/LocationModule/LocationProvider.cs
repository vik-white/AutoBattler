using vikwhite.Data;

namespace vikwhite
{
    public interface ILocationProvider
    {
        string ID { get; set; }
        LocationType Type { get; set; }
    }
    
    public class LocationProvider : ILocationProvider
    {
        public string ID { get; set; }
        public LocationType Type { get; set; }
    }
}