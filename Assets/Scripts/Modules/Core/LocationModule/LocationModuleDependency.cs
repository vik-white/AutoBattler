namespace vikwhite
{
    public class LocationModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<ILocationProvider, LocationProvider>();
        }
    }
}