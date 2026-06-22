namespace vikwhite
{
    public class CheatModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<ICheatWindow, CheatWindow>();
            Register<ICheatService, CheatService>();
            Register<CheatWindowViewModel>();
            Register<CheatWindowView>();
            Register<IMapItemViewFactory, MapItemViewFactory>();
            Register<MapItemViewModel>();
            Register<MapItemView>();
        }
    }
}
