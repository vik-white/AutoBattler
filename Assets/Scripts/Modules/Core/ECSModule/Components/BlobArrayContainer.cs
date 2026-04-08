using Unity.Entities;

namespace vikwhite
{
    public struct BlobArrayContainer<T> where T : unmanaged
    {
        public BlobArray<T> Array;
    }
}