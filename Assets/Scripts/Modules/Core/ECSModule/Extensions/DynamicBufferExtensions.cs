using System;
using Unity.Entities;

namespace vikwhite
{
    public interface IID
    {
        uint ID { get; set; }
    }
    
    public static class DynamicBufferExtensions
    {
        public static T Get<T>(this DynamicBuffer<T> buffer, uint id) where T : unmanaged, IID
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].ID == id)
                {
                    return buffer[i];
                }
            }
            return default;
        }
    }

    public static class BlobArrayExtensions
    {
        public static ref T Get<T>(this BlobAssetReference<BlobArrayContainer<T>> buffer, uint id) where T : unmanaged, IID
        {
            for (int i = 0; i < buffer.Value.Array.Length; i++)
            {
                if (buffer.Value.Array[i].ID == id)
                { 
                    return ref buffer.Value.Array[i];
                }
            }

            throw new InvalidOperationException($"Blob element with id {id} was not found.");
        }
    }
}
