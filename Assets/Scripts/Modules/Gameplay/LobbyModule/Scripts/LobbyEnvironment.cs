using System.Collections;
using UnityEngine.SceneManagement;

namespace vikwhite
{
    public class LobbyEnvironment : Environment
    {
        private const string TavernScene = "Tavern";

        private Scene _previousActiveScene;

        protected override void Register()
        {
            Register<LoadingScreenModuleDependency>();
            Register<LobbyModuleDependency>();
            Register<CheatModuleDependency>();
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
            var loadingScreen = Resolve<ILoadingScreenService>();
            loadingScreen.Show();
            yield return null;
            Resolve<ICheatService>();
            Resolve<IProfileService>().Load(); 
            Resolve<IResourceService>().Initialize();
            Resolve<ICharactersService>().Initialize();
            Resolve<ISectorService>().Initialize();
            Resolve<IEventsService>().Initialize();
            Resolve<IRoomsService>().Initialize();
            _previousActiveScene = SceneManager.GetActiveScene();
            var loader = SceneManager.LoadSceneAsync(TavernScene, LoadSceneMode.Additive);
            yield return loadingScreen.TrackProgress(loader);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(TavernScene));
            yield return loadingScreen.Hide();
            Resolve<IStateMachine<ILobbyState>>().SwitchState<ILobbyStartState>();
            yield return null;
        }

        protected override void Release()
        {
            Resolve<IStateMachine<ILobbyState>>().SwitchState<ILobbyEndState>();
            SceneManager.SetActiveScene(_previousActiveScene);
            SceneManager.UnloadSceneAsync(TavernScene);
        }
    }
}
