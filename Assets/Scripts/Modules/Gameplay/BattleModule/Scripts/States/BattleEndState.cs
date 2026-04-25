namespace vikwhite
{
    public interface IBattleEndState : IBattleState { }
    
    public class BattleEndState : IBattleEndState
    {
        public void Enter() { }

        public void Exit() { }
    }
}