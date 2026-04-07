namespace vikwhite
{
    public class EnvironmentModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IEnvironmentStateFactory, EnvironmentStateFactory>();
            Register<IEnvironmentStateMachine, EnvironmentStateMachine>();
        }
    }
}