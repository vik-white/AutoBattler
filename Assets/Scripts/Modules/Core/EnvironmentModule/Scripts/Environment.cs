using System;
using System.Collections;

namespace vikwhite
{
    public class Environment : IDisposable
    {
        private readonly IDiContainer _container = DI.Create();
        
        public void Load()
        {
            Register();
            CoroutineRunner.Instance.StartCoroutine(Initialize());
        }
        
        protected void Register<T>() where T : DiModule, new() => new T().Initialize(_container);
        
        protected T Resolve<T>() => _container.Resolve<T>();
        
        protected virtual void Register() { }
        
        protected virtual IEnumerator Initialize() { 
            yield return null;
        }
        
        protected virtual void Release() { }

        public virtual void Dispose()
        {
            Release();
            _container.Dispose();
        }
    }
}