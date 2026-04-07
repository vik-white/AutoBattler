using Unity.Entities;
using vikwhite;

namespace Utilities.Extensions
{
    public static class EntityManagerExtensions
    {
        public static Entity CreateFrameEntity<T>(this EntityManager entityManager, in T component) where T : unmanaged, IComponentData {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, component);
            entityManager.AddComponent<Destroy>(entity);
            return entity;
        }
    }
}