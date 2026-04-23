namespace vikwhite
{
    public interface IEnvironmentStateMachine
    {
        void SwitchState(EnvironmentType key);
    }
    
    public class EnvironmentStateMachine : IEnvironmentStateMachine
    {
        private readonly IEnvironmentStateFactory _factory;
        private readonly IWindowManager _windowManager;
        private IEnvironmentState _currentState;

        public EnvironmentStateMachine(IEnvironmentStateFactory factory, IWindowManager windowManager)
        {
            _factory = factory;
            _windowManager = windowManager;
        }

        public void SwitchState(EnvironmentType key)
        {
            _windowManager.CloseAllWindows();
            _currentState?.Exit();
            _currentState = _factory.Get(key);
            _currentState.Enter();
        }
    }
}
