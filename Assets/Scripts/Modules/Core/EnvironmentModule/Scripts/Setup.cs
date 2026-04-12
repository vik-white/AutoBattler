using vikwhite.Data;

namespace vikwhite
{
    public class Setup
    {
        private readonly IEnvironmentStateFactory _stateFactory;
        private readonly IEnvironmentStateMachine _stateMachine;

        public Setup()
        {
            new CoreEnvironment().Load();
            _stateFactory = DI.Resolve<IEnvironmentStateFactory>();
            _stateMachine = DI.Resolve<IEnvironmentStateMachine>();
        }

        public Setup Configs(IConfigs configs)
        {
            DI.Register<IConfigs>(configs);
            return this;
        }

        public Setup Add<T>(EnvironmentType type) where T : Environment, new()
        {
            _stateFactory.Add<T>(type, new T());
            return this;
        }

        public void Start(EnvironmentType type)
        {
            _stateMachine.SwitchState(type);
        }
    }
}