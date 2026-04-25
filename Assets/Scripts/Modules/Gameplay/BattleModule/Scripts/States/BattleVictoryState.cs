namespace vikwhite
{
    public interface IBattleVictoryState : IBattleState { }
    
    public class BattleVictoryState : IBattleVictoryState
    {
        private readonly IVictoryWindow _victoryWindow;
        
        public BattleVictoryState(IVictoryWindow victoryWindow)
        {
            _victoryWindow = victoryWindow;
        }

        public void Enter()
        {
            _victoryWindow.ShowWindow();
        }

        public void Exit() { }
    }
}