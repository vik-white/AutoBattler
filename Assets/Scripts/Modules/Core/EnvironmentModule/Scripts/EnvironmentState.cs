namespace vikwhite
{
    public interface IEnvironmentState : IState
    {
    }
    
    public interface IEnvironmentState<T> : IEnvironmentState where T : Environment
    {
    }

    public class EnvironmentState<T> : IEnvironmentState<T> where T : Environment
    {
        private readonly Environment _environment;

        public EnvironmentState(Environment environment)
        {
            _environment = environment;
        }

        public virtual void Enter()
        {
            _environment.Load();
        }
        
        public virtual void Exit()
        {
            _environment.Dispose();
        }
    }
}