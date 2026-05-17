using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vikwhite
{
    public class SectorEnvironment : Environment
    {
        protected override void Register()
        {
            Register<SectorModuleDependency>();
            Register<SquadModuleDependency>();
            Register<ProfileModuleDependency>();
            Register<ResourceModuleDependency>();
            Register<RoadMapModuleDependency>();
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
            Resolve<IRoadMapService>().Initialize();
            var loader = SceneManager.LoadSceneAsync("Sector1", LoadSceneMode.Additive);
            while (!loader.isDone) yield return null;
            yield return new WaitForSeconds(0.1f);
            Resolve<ISectorMapService>().Initialize();
            Resolve<IStateMachine<ISectorState>>().SwitchState<ISectorStartState>();
            yield return null;
        }

        protected override void Release()
        {
            Resolve<IStateMachine<ISectorState>>().SwitchState<ISectorEndState>();
            SceneManager.UnloadSceneAsync("Sector1");
        }
    }
}
