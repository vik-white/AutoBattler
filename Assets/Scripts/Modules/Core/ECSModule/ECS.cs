using Unity.Entities;

namespace vikwhite
{
    public static class ECSWorld
    {
        private static World _world = World.DefaultGameObjectInjectionWorld;
        
        public static void Enable<T>() where T : unmanaged, ISystem
        {
            _world.Unmanaged.ResolveSystemStateRef(_world.GetExistingSystem<T>()).Enabled = true;
        }

        public static void DestroyScene()
        {
            Entity entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponentData(entity, new DestroyScene());
        }
        
        public static void CreateEntity<T>(T component) where T : unmanaged, IComponentData
        {
            Entity entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddComponentData(entity, component);
        }
    }
}