namespace vikwhite
{
    public class CheatModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<ICheatWindow, CheatWindow>();
            Register<ICheatWindowHotkey, CheatWindowHotkey>();
            Register<CheatWindowViewModel>();
            Register<CheatWindowView>();
            Register<IMapItemViewFactory, MapItemViewFactory>();
            Register<MapItemViewModel>();
            Register<MapItemView>();

            _container.Resolve<ICheatWindowHotkey>();
        }
    }
}
