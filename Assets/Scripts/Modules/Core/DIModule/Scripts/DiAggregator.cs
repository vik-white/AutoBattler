using System;
using System.Collections.Generic;

namespace vikwhite
{
    public class DiAggregator
    {
        private List<DiContainer> _containers = new ();
        
        public void Add(DiContainer container) => _containers.Add(container);
        
        public void Update()
        {
            foreach (var c in _containers) c.Update();
        }
        
        public void Register<TInterface, TImplementation>() => _containers[0].Register<TInterface, TImplementation>();
        
        public object Resolve(Type type)
        {
            foreach (var c in _containers) {
                if (c.CanResolve(type)) return c.ResolveCurrent(type);
            }
            throw new Exception($"Type not registered: {type}");
        }
        
        public object Resolve(Type type, object arg)
        {
            foreach (var c in _containers) {
                if (c.CanResolve(type)) return c.ResolveCurrent(type, arg);
            }
            throw new Exception($"Type not registered: {type}");
        }

        internal object ResolveExternal(Type type, DiContainer requester, object arg)
        {
            foreach (var c in _containers) {
                if (c == requester) continue;
                if (c.CanResolve(type)) return c.ResolveCurrent(type, arg);
            }
            throw new Exception($"Type not registered: {type}");
        }

        internal List<object> ResolveAll(Type type)
        {
            var result = new List<object>();
            foreach (var c in _containers) {
                if (!c.CanResolve(type)) continue;
                result.Add(c.ResolveCurrent(type));
            }
            return result;
        }
    }
}