namespace vikwhite
{
    public class CameraModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<ICameraService, CameraService>();
        }
    }
}
