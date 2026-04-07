using System;
using System.Collections.Generic;

namespace vikwhite
{
    public class Entity
    {
        private readonly Dictionary<Type, Component> _components = new();
        
        public void Add<T>(T component) where T : Component
        {
            _components[typeof(T)] = component;
        }
        
        public T Get<T>() where T : Component => (T)_components[typeof(T)];
        public (T1, T2) Get<T1, T2>() where T1 : Component where T2 : Component => (Get<T1>(), Get<T2>());
        public (T1, T2, T3) Get<T1, T2, T3>() where T1 : Component where T2 : Component where T3 : Component => (Get<T1>(), Get<T2>(), Get<T3>());
    }
}