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
}