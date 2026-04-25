namespace vikwhite
{
    public interface IBattleDefeatState : IBattleState
    {
    }

    public class BattleDefeatState : IBattleDefeatState
    {
        private readonly IDefeatWindow _defeatWindow;
        
        public BattleDefeatState(IDefeatWindow defeatWindow)
        {
            _defeatWindow = defeatWindow;
        }

        public void Enter()
        {
            _defeatWindow.ShowWindow();
        }

        public void Exit() { }
    }
}