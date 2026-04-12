namespace vikwhite
{
    public interface ILocationProvider
    {
        string Location { get; set; }
    }
    
    public class LocationProvider : ILocationProvider
    {
        public string Location { get; set; }
    }
}