using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace vikwhite
{
    public class BattleEnvironment : Environment
    {
        protected override void Register()
        {
            Register<BattleModuleDependency>();
            Register<ProfileModuleDependency>();
            Register<SquadModuleDependency>();
        }

        protected override IEnumerator Initialize()
        {
            Resolve<IProfileService>().Load(); 
            Resolve<ISquad>().Initialize();
            var loader = SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Additive);
            while (!loader.isDone) yield return null;
            yield return new WaitForSeconds(0.1f);
            Resolve<IStateMachine<IBattleState>>().SwitchState<IBattleStartState>();
        }
        
        protected override void Release()
        {
            Resolve<IStateMachine<IBattleState>>().SwitchState<IBattleEndState>();
            SceneManager.UnloadSceneAsync("Battle");
        }
    }
}