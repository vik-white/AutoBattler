namespace vikwhite
{
    public class RoadMapModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IRoadMapService, RoadMapService>();
        }
    }
}