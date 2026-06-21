using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vikwhite
{
    public class BattleEnvironment : Environment
    {
        protected override void Register()
        {
            Register<LoadingScreenModuleDependency>();
            Register<BattleModuleDependency>();
            Register<ProfileModuleDependency>();
            Register<SquadModuleDependency>();
            Register<ResourceModuleDependency>();
            Register<CharacterModuleDependency>();
            Register<SectorModuleDependency>();
            Register<RewardModuleDependency>();
            Register<QuestsModuleDependency>();
            Register<EventsModuleDependency>();
            Register<CheatModuleDependency>();
        }

        protected override IEnumerator Initialize()
        {
            var loadingScreen = Resolve<ILoadingScreenService>();
            loadingScreen.Show();
            yield return null;
            Resolve<IProfileService>().Load(); 
            Resolve<IResourceService>().Initialize();
            Resolve<IClassShardService>().Initialize();
            Resolve<IClassBookService>().Initialize();
            Resolve<ISectorService>().Initialize();
            Resolve<ICharactersService>().Initialize();
            Resolve<ISquadService>().Initialize();
            Resolve<IEventsService>().Initialize();
            Resolve<IEventsService>().Initialize();
            var loader = SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Additive);
            yield return loadingScreen.TrackProgress(loader);
            yield return loadingScreen.Hide();
            Resolve<IStateMachine<IBattleState>>().SwitchState<IBattleStartState>();
        }
        
        protected override void Release()
        {
            Resolve<IStateMachine<IBattleState>>().SwitchState<IBattleEndState>();
            SceneManager.UnloadSceneAsync("Battle");
        }
    }
}
