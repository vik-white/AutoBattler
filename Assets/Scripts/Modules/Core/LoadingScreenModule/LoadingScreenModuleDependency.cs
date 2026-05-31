namespace vikwhite
{
    public class LoadingScreenModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<ILoadingScreenWindow, LoadingScreenWindow>();
            Register<LoadingScreenViewModel>();
            Register<LoadingScreenView>();
            Register<ILoadingScreenService, LoadingScreenService>();
        }
    }
}
