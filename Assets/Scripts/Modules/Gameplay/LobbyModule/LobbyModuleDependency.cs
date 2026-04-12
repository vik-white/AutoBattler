namespace vikwhite
{
    public class LobbyModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IStateMachine<ILobbyState>, StateMachine<ILobbyState>>();
            Register<IStateFactory<ILobbyState>, StateFactory<ILobbyState>>();
            Register<ILobbyStartState, LobbyStartState>();
            Register<ILobbyEndState, LobbyEndState>();
            
            Register<IMapWindow, MapWindow>();
            Register<MapWindowViewModel>();
            Register<MapWindowView>();
            Register<IMapItemViewFactory, MapItemViewFactory>();
            Register<MapItemViewModel>();
            Register<MapItemView>();
        }
    }
}