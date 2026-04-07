namespace vikwhite
{
    public class DiContext : DiRegister
    {
        protected void Register<T>() where T : DiModule, new() => new T().Initialize(_container);
    }
}