using UnityEngine.SceneManagement;
using vikwhite.ECS;

namespace vikwhite
{
    public class BattleEnvironment : Environment
    {
        protected override void Register()
        {
            Register<BattleModuleDependency>();
        }

        protected override void Initialize()
        {
            SceneManager.LoadScene("Battle", LoadSceneMode.Additive);
            ECSWorld.Enable<SpawnCharacterSystem>();
            Resolve<IStateMachine<IBattleState>>().SwitchState<IBattleStartState>();
        }
        
        protected override void Release()
        {
            SceneManager.UnloadSceneAsync("Battle");
            ECSWorld.DestroyScene();
        }
    }
}