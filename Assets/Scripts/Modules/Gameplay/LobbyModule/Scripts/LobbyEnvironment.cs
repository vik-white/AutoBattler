namespace vikwhite
{
    public class LobbyEnvironment : Environment
    {
        protected override void Register()
        {
            Register<LobbyModuleDependency>();
            Register<ProfileModuleDependency>();
        }

        protected override void Initialize()
        {
            Resolve<IStateMachine<ILobbyState>>().SwitchState<ILobbyStartState>();
        }
    }
}