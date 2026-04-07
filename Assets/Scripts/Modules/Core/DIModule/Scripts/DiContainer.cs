using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace vikwhite
{
    public interface IUpdatable
    {
        void Update();
    }
    
    public interface IDiContainer : IDisposable
    {
        void Register<TInterface, TImplementation>();
        void Register<TImplementation>();
        void Register<TInterface>(object implementation);
        T Resolve<T>();
        T Resolve<T, TArg>(TArg arg);
    }
    
    public class DiContainer : IDiContainer
    {
        protected readonly DiAggregator _aggregator;
        protected readonly Dictionary<Type, List<Type>> _typeMap = new();
        protected readonly Dictionary<Type, object> _singletons = new();
        protected readonly HashSet<Type> _resolving = new();

        public DiContainer(DiAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.Add(this);
            Register<DiContainer>(this);
        }
        
        public void Update()
        {
            foreach (var key in _typeMap.Keys.ToList()) {
                if (_singletons.ContainsKey(key) && _singletons[key] is IUpdatable updatable) 
                    updatable.Update();
            }
        }

        public void Register<TInterface, TImplementation>()
        {
            var interfaceType = typeof(TInterface);
            if (!_typeMap.TryGetValue(interfaceType, out var list)) {
                list = new List<Type>();
                _typeMap[interfaceType] = list;
                _singletons[interfaceType] = null;
            }
            list.Add(typeof(TImplementation));
        }

        public void Register<TImplementation>()
        {
            var type = typeof(TImplementation);
            if (!_typeMap.ContainsKey(type)) _typeMap[type] = new List<Type>();
            _typeMap[type].Add(type);
        }
        
        public void Register<TInterface>(object implementation)
        {
            _typeMap[typeof(TInterface)] = new List<Type> { typeof(TInterface) };
            _singletons[typeof(TInterface)] = implementation;
        }

        public bool CanResolve(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
                var itemType = type.GetGenericArguments()[0];
                return _typeMap.ContainsKey(itemType);
            }
            return _typeMap.ContainsKey(type) || _singletons.ContainsKey(type);
        }

        public T Resolve<T>() => (T) _aggregator.Resolve(typeof(T));
        
        public object Resolve(Type type) => _aggregator.Resolve(type);
        
        public object Resolve(Type type, object arg) => _aggregator.Resolve(type, arg);

        public T Resolve<T, TArg>(TArg arg) => (T) _aggregator.Resolve(typeof(T), arg);

        public object ResolveCurrent(Type type, object externalArg = null)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var itemType = type.GetGenericArguments()[0];

                var all = _aggregator.ResolveAll(itemType);

                var array = Array.CreateInstance(itemType, all.Count);

                for (int i = 0; i < all.Count; i++)
                    array.SetValue(all[i], i);

                return array;
            }

            if (_singletons.TryGetValue(type, out var singleton))
            {
                if (singleton == null)
                {
                    var impl = _typeMap[type].First();
                    singleton = CreateInstance(impl, externalArg);
                    _singletons[type] = singleton;
                }

                return singleton;
            }

            if (_typeMap.TryGetValue(type, out var implList))
                type = implList.First();
            else
                return _aggregator.ResolveExternal(type, this, externalArg);

            return CreateInstance(type, externalArg);
        }

        private object CreateInstance(Type type, object externalArg = null)
        {
            if (_resolving.Contains(type))
                throw new Exception($"Cyclic dependency detected: {type}");

            _resolving.Add(type);

            try
            {
                var constructor = type
                    .GetConstructors()
                    .OrderByDescending(c => c.GetParameters().Length)
                    .First();

                var args = constructor
                    .GetParameters()
                    .Select(p =>
                        externalArg != null &&
                        p.ParameterType.IsInstanceOfType(externalArg)
                            ? externalArg
                            : ResolveCurrent(p.ParameterType))
                    .ToArray();

                return Activator.CreateInstance(type, args);
            }
            finally
            {
                _resolving.Remove(type);
            }
        }

        public void Dispose()
        {
            _typeMap.Clear();
            _singletons.Clear();
            _resolving.Clear();
        }
    }
}