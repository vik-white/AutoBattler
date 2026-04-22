using System.Collections;

namespace vikwhite
{
    public class LobbyEnvironment : Environment
    {
        protected override void Register()
        {
            Register<LobbyModuleDependency>();
            Register<CheatModuleDependency>();
            Register<SquadModuleDependency>();
            Register<ProfileModuleDependency>();
        }

        protected override IEnumerator Initialize()
        {
            Resolve<IProfileService>().Load(); 
            Resolve<ISquad>().Initialize();
            Resolve<IStateMachine<ILobbyState>>().SwitchState<ILobbyStartState>();
            yield return null;
        }

        protected override void Release()
        {
            Resolve<IStateMachine<ILobbyState>>().SwitchState<ILobbyEndState>();
        }
    }
}