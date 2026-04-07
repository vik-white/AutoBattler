namespace vikwhite
{
    public class LobbyModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IStateMachine<ILobbyState>, StateMachine<ILobbyState>>();
            Register<IStateFactory<ILobbyState>, StateFactory<ILobbyState>>();
            Register<ILobbyStartState, LobbyStartState>();
        }
    }
}