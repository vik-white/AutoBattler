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
            Register<SectorModuleDependency>();
            Register<SquadModuleDependency>();
            Register<ProfileModuleDependency>();
            Register<ResourceModuleDependency>();
            Register<CharacterModuleDependency>();
        }

        protected override IEnumerator Initialize()
        {
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
            while (!loader.isDone) yield return null;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sectorScene));
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
