using UnityEngine.SceneManagement;

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
            Resolve<IStateMachine<IBattleState>>().SwitchState<IBattleStartState>();
        }
        
        protected override void Release()
        {
            Resolve<IStateMachine<IBattleState>>().SwitchState<IBattleEndState>();
            SceneManager.UnloadSceneAsync("Battle");
        }
    }
}