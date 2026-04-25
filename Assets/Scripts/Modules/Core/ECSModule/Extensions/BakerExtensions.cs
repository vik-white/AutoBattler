using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace vikwhite
{
    public static class BakerExtensions
    {
        static HashSet<Entity> initialized = new();
        static Dictionary<GameObject, int> cache = new();

        public static int RegisterPrefab<T>(this Baker<T> baker, GameObject prefab) where T : Component
        {
            if (prefab == null) return -1;

            if (cache.TryGetValue(prefab, out var existing))
                return existing;

            var registryAuthoring = Object.FindAnyObjectByType<PrefabRegistry>();
            if (registryAuthoring == null) return -1;

            var registryEntity = baker.GetEntity(registryAuthoring, TransformUsageFlags.None);
            var prefabEntity = baker.GetEntity(prefab, TransformUsageFlags.Dynamic);

            if (!initialized.Contains(registryEntity))
            {
                baker.AddBuffer<Prefab>(registryEntity);
                initialized.Add(registryEntity);
            }

            int index = cache.Count;
            cache[prefab] = index;

            baker.AppendToBuffer(registryEntity, new Prefab { Value = prefabEntity });

            return index;
        }
    }
}
