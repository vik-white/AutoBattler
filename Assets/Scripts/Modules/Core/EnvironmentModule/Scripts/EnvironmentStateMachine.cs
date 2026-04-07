namespace vikwhite
{
    public interface IEnvironmentStateMachine
    {
        void SwitchState(EnvironmentType key);
    }
    
    public class EnvironmentStateMachine : IEnvironmentStateMachine
    {
        private readonly IEnvironmentStateFactory _factory;
        private IEnvironmentState _currentState;

        public EnvironmentStateMachine(IEnvironmentStateFactory factory)
        {
            _factory = factory;
        }

        public void SwitchState(EnvironmentType key)
        {
            _currentState?.Exit();
            _currentState = _factory.Get(key);
            _currentState.Enter();
        }
    }
}