using Unity.Entities;

namespace Utilities.Extensions
{
    public static class WorldExtensions
    {
        public static bool TryGetEntityManager(this World world, out EntityManager entityManager)
        {
            if (world == null)
            {
                entityManager = default;
                return false;
            }

            entityManager = world.EntityManager;
            return true;
        }
    }
}
