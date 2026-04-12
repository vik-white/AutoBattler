using UnityEngine;

namespace vikwhite
{
    public static class DI
    {
        private static DiAggregator _aggregator;
        
        public static DiContainer Create()
        {
            if (_aggregator == null) {
                _aggregator = new DiAggregator();
                var gameObject = new GameObject("DiSceneContext");
                var sceneContext =  gameObject.AddComponent<DiSceneContext>();
                sceneContext.Initialize(_aggregator);
            }
            return new DiContainer(_aggregator);
        }
        
        public static void Register<TInterface, TImplementation>() => _aggregator.Register<TInterface, TImplementation>();
        
        public static void Register<TInterface>(object implementation) => _aggregator.Register<TInterface>(implementation);
        
        public static T Resolve<T>() => (T)_aggregator.Resolve(typeof(T));
    }
}