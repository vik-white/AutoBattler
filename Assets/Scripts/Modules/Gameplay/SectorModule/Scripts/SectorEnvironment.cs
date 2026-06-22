using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vikwhite
{
    public class SectorEnvironment : Environment
    {
        private string _sectorScene;
        private Scene _previousActiveScene;

        protected override void Register()
        {
            Register<LoadingScreenModuleDependency>();
            Register<SectorModuleDependency>();
            Register<SquadModuleDependency>();
            Register<ProfileModuleDependency>();
            Register<ResourceModuleDependency>();
            Register<CharacterModuleDependency>();
            Register<CheatModuleDependency>();
        }

        protected override IEnumerator Initialize()
        {
            var loadingScreen = Resolve<ILoadingScreenService>();
            loadingScreen.Show();
            yield return null;
            Resolve<ICheatService>();
            Resolve<IProfileService>().Load();
            Resolve<IResourceService>().Initialize();
            Resolve<IClassShardService>().Initialize();
            Resolve<IClassBookService>().Initialize();
            Resolve<ICharactersService>().Initialize();
            Resolve<ISquadService>().Initialize();
            var sector = Resolve<ISectorService>();
            sector.Initialize();
            _sectorScene = sector.CurrentSector;
            _previousActiveScene = SceneManager.GetActiveScene();
            var loader = SceneManager.LoadSceneAsync(_sectorScene, LoadSceneMode.Additive);
            yield return loadingScreen.TrackProgress(loader);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sectorScene));
            yield return loadingScreen.Hide();
            Resolve<IStateMachine<ISectorState>>().SwitchState<ISectorStartState>();
            yield return null;
        }

        protected override void Release()
        {
            Resolve<IStateMachine<ISectorState>>().SwitchState<ISectorEndState>();
            SceneManager.SetActiveScene(_previousActiveScene);
            SceneManager.UnloadSceneAsync(_sectorScene);
        }
    }
}
