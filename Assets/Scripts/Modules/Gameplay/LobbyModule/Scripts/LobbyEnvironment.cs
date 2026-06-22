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
            Register<ResourceModuleDependency>();
            Register<SectorModuleDependency>();
            Register<CharacterModuleDependency>();
            Register<RewardModuleDependency>();
            Register<BankModuleDependency>();
            Register<MetaModuleDependency>();
            Register<QuestsModuleDependency>();
            Register<EventsModuleDependency>();
        }

        protected override IEnumerator Initialize()
        {
            Resolve<ICheatService>();
            Resolve<IProfileService>().Load(); 
            Resolve<IResourceService>().Initialize();
            Resolve<IClassShardService>().Initialize();
            Resolve<IClassBookService>().Initialize();
            Resolve<ICharactersService>().Initialize();
            Resolve<ISquadService>().Initialize();
            Resolve<ISectorService>().Initialize();
            Resolve<IEventsService>().Initialize();
            Resolve<IStateMachine<ILobbyState>>().SwitchState<ILobbyStartState>();
            yield return null;
        }

        protected override void Release()
        {
            Resolve<IStateMachine<ILobbyState>>().SwitchState<ILobbyEndState>();
        }
    }
}
