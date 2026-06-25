namespace vikwhite
{
    public class SquadModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<ISquadService, SquadService>();
            Register<ISquadWindow, SquadWindow>();
            Register<SquadWindowViewModel>();
            Register<SquadWindowView>();
            
            Register<ISquadItemViewFactory, SquadItemViewFactory>();
            Register<SquadItemViewModel>();
            Register<SquadItemView>();
        }
    }
}
