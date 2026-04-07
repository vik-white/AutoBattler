namespace vikwhite
{
    public class DiModule : DiRegister
    {
        protected void Register<T>() => _container.Register<T>();
        
        protected void Register<T,I>() => _container.Register<T,I>();
    }
}