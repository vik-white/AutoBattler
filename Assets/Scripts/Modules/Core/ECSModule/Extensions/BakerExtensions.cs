using Unity.Entities;
using UnityEngine;

namespace vikwhite
{
    public static class BakerExtensions
    {
        public static int RegisterPrefab<T>(this Baker<T> baker, GameObject prefab) where T : Component 
        {
            if (prefab == null) return -1;

            var registryAuthoring = Object.FindFirstObjectByType<PrefabRegistry>();
            if (registryAuthoring == null) return -1;

            var registryEntity = baker.GetEntity(registryAuthoring);
        
            DynamicBuffer<Prefab> buffer;
        
            try 
            {
                buffer = baker.AddBuffer<Prefab>(registryEntity);
            }
            catch 
            {
                buffer = baker.SetBuffer<Prefab>(registryEntity);
            }

            var prefabEntity = baker.GetEntity(prefab, TransformUsageFlags.Dynamic);

            for (int i = 0; i < buffer.Length; i++) {
                if (buffer[i].Value == prefabEntity) return i;
            }

            int index = buffer.Length;
            buffer.Add(new Prefab { Value = prefabEntity });
            return index;
        }
    }
}