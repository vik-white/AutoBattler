namespace vikwhite
{
    public interface IState : IExitableState
    {
        public void Enter();
    }
    
    public interface IExitableState
    {
        public void Exit();
    }
    
    public interface IUpdateble
    {
        public void Update();
    }
}