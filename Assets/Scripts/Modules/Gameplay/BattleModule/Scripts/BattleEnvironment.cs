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
            Resolve<IStateMachine<IBattleState>>().SwitchState<IBattleStartState>();
        }
    }
}