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
    }
}