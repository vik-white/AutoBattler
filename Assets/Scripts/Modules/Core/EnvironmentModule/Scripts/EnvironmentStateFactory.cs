using System;
using System.Collections.Generic;

namespace vikwhite
{
    public interface IEnvironmentStateFactory
    {
        void Add<T>(EnvironmentType key, Environment environment) where T : Environment;
        IEnvironmentState Get(EnvironmentType key);
    }
    
    public class EnvironmentStateFactory : IEnvironmentStateFactory
    {
        private readonly DiContainer _container;
        private readonly Dictionary<EnvironmentType, Type> _types = new ();
        private readonly Dictionary<EnvironmentType, Environment> _environments = new ();

        public EnvironmentStateFactory(DiContainer container) {
            _container = container;
        }

        public void Add<T>(EnvironmentType key, Environment environment) where T : Environment
        {
            _container.Register<IEnvironmentState<T>, EnvironmentState<T>>();
            _types.Add(key, typeof(IEnvironmentState<T>));
            _environments.Add(key, environment);
        }

        public IEnvironmentState Get(EnvironmentType key)
        {
            return (IEnvironmentState)_container.Resolve(_types[key], _environments[key]);
        }
    }
}