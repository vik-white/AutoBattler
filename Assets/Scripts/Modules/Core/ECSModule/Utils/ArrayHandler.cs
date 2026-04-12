using System;
using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public class ArrayHandler
    {
        public static BlobAssetReference<BlobArrayContainer<T>> CreateBlobArray<T>(int length, Func<int, T> elementFactory) where T : unmanaged {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<BlobArrayContainer<T>>();
            var arrayBuilder = builder.Allocate(ref root.Array, length);
            for (int i = 0; i < length; i++) arrayBuilder[i] = elementFactory(i);
            return builder.CreateBlobAssetReference<BlobArrayContainer<T>>(Allocator.Persistent);
        }
        
        public static FixedList128Bytes<float> CreateArray128Bytes(int count, float value) {
            var array = new FixedList128Bytes<float>();
            for (int i = 0; i < count; i++) array.Add(value);
            return array;
        }
        
        public static FixedList4096Bytes<bool> CreateArray4096Bytes(int count, bool value) {
            var array = new FixedList4096Bytes<bool>();
            for (int i = 0; i < count; i++) array.Add(value);
            return array;
        }
    }
}