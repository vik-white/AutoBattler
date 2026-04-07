using System;

namespace vikwhite
{
    public class Environment : IDisposable
    {
        private readonly IDiContainer _container = DI.Create();
        
        public void Load()
        {
            Register();
            Initialize();
        }
        
        protected void Register<T>() where T : DiModule, new() => new T().Initialize(_container);
        
        protected T Resolve<T>() => _container.Resolve<T>();
        
        protected virtual void Register() { }
        
        protected virtual void Initialize() { }

        public virtual void Dispose()
        {
            _container.Dispose();
        }
    }
}