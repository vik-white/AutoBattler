namespace vikwhite
{
    public class BattleModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IStateMachine<IBattleState>, StateMachine<IBattleState>>();
            Register<IStateFactory<IBattleState>, StateFactory<IBattleState>>();
            Register<IBattleStartState, BattleStartState>();
            Register<IBattleDefeatState, BattleDefeatState>();
            Register<IBattleVictoryState, BattleVictoryState>();
            Register<IBattleEndState, BattleEndState>();
            Register<IBattleSquadPlacementService, BattleSquadPlacementService>();
            Register<IBattleMightService, BattleMightService>();

            Register<IBattleWindow, BattleWindow>();
            Register<BattleWindowViewModel>();
            Register<BattleWindowView>();
            Register<IBattleSkillViewFactory, BattleSkillViewFactory>();
            Register<BattleSkillViewModel>();
            Register<BattleSkillView>();
            Register<IBattleHealthBarViewFactory, BattleHealthBarViewFactory>();
            Register<BattleHealthBarViewModel>();
            Register<BattleHealthBarView>();
            Register<IBattleDamageFlyTextViewFactory, BattleDamageFlyTextViewFactory>();
            Register<BattleDamageFlyTextViewModel>();
            Register<BattleDamageFlyTextView>();
            
            Register<IVictoryWindow, VictoryWindow>();
            Register<VictoryWindowViewModel>();
            Register<VictoryWindowView>();
            
            Register<IDefeatWindow, DefeatWindow>();
            Register<DefeatWindowViewModel>();
            Register<DefeatWindowView>();
        }
    }
}
