using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vikwhite
{
    public class SectorEnvironment : Environment
    {
        private string _sectorScene;
        private ISectorPlayerPresenter _playerPresenter;

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
            if (string.IsNullOrEmpty(_sectorScene))
            {
                Debug.LogError($"Sector scene was not found for location {sector.CurrentLocation}.");
                yield break;
            }

            var loader = SceneManager.LoadSceneAsync(_sectorScene, LoadSceneMode.Additive);
            while (!loader.isDone) yield return null;
            yield return new WaitForSeconds(0.1f);
            _playerPresenter = Resolve<ISectorPlayerPresenter>();
            _playerPresenter.Initialize();
            sector.InitializePoints();
            Resolve<IStateMachine<ISectorState>>().SwitchState<ISectorStartState>();
            yield return null;
        }

        protected override void Release()
        {
            _playerPresenter?.Release();
            _playerPresenter = null;
            Resolve<IStateMachine<ISectorState>>().SwitchState<ISectorEndState>();
            if (!string.IsNullOrEmpty(_sectorScene)) SceneManager.UnloadSceneAsync(_sectorScene);
        }
    }
}
