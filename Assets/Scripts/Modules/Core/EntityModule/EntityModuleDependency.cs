namespace vikwhite
{
    public class EntityModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<Entity>();
        }
    }
}