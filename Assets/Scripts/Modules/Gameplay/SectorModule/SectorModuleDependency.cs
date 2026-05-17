namespace vikwhite
{
    public class SectorModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IStateMachine<ISectorState>, StateMachine<ISectorState>>();
            Register<IStateFactory<ISectorState>, StateFactory<ISectorState>>();
            Register<ISectorStartState, SectorStartState>();
            Register<ISectorEndState, SectorEndState>();

            Register<ISectorWindow, SectorWindow>();
            Register<SectorWindowViewModel>();
            Register<SectorWindowView>();
            Register<ISectorService, SectorService>();
        }
    }
}
