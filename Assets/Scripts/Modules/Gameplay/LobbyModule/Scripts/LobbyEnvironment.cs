using System.Collections;

namespace vikwhite
{
    public class LobbyEnvironment : Environment
    {
        protected override void Register()
        {
            Register<LobbyModuleDependency>();
            Register<ProfileModuleDependency>();
            Register<CheatModuleDependency>();
            Register<SquadModuleDependency>();
        }

        protected override IEnumerator Initialize()
        {
            Resolve<IStateMachine<ILobbyState>>().SwitchState<ILobbyStartState>();
            yield return null;
        }

        protected override void Release()
        {
            Resolve<IStateMachine<ILobbyState>>().SwitchState<ILobbyEndState>();
        }
    }
}