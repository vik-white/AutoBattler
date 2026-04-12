namespace vikwhite
{
    public class BattleModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IStateMachine<IBattleState>, StateMachine<IBattleState>>();
            Register<IStateFactory<IBattleState>, StateFactory<IBattleState>>();
            Register<IBattleStartState, BattleStartState>();
            Register<IBattleEndState, BattleEndState>();
        }
    }
}