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
            var roadMap = Resolve<IRoadMapService>();
            roadMap.Initialize();

            var loader = SceneManager.LoadSceneAsync(roadMap.CurrentSector, LoadSceneMode.Additive);
            while (!loader.isDone) yield return null;
            yield return new WaitForSeconds(0.1f);
            Resolve<ISectorService>().Initialize(roadMap.CurrentSector);
            Resolve<IStateMachine<ISectorState>>().SwitchState<ISectorStartState>();
            yield return null;
        }

        protected override void Release()
        {
            var roadMap = Resolve<IRoadMapService>();
            Resolve<IStateMachine<ISectorState>>().SwitchState<ISectorEndState>();
            SceneManager.UnloadSceneAsync(roadMap.CurrentSector);
        }
    }
}
