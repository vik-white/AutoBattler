using System.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    public static class ECSWorld
    {
        public static void SetEnabled<T>(bool enabled) where T : unmanaged, ISystem
        {
            var world = World.DefaultGameObjectInjectionWorld;
            world.Unmanaged.ResolveSystemStateRef(world.GetExistingSystem<T>()).Enabled = enabled;
        }

        public static void SetManagedEnabled<T>(bool enabled) where T : ComponentSystemBase
        {
            var world = World.DefaultGameObjectInjectionWorld;
            world.GetExistingSystemManaged<T>().Enabled = enabled;
        }

        public static void DestroyScene()
        {
            CoroutineRunner.Instance.StartCoroutine(DestroyNextFrame());
        }
        
        private static IEnumerator DestroyNextFrame()
        {
            yield return null;
            var world = World.DefaultGameObjectInjectionWorld;
            Entity entity = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponentData(entity, new DestroyScene());
        }
        
        public static void CreateEntity<T>(T component) where T : unmanaged, IComponentData
        {
            var world = World.DefaultGameObjectInjectionWorld;
            Entity entity = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponentData(entity, new SceneEntity());
            world.EntityManager.AddComponentData(entity, component);
        }
    }
}
