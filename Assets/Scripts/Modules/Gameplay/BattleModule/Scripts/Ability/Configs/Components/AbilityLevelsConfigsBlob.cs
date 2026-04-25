using Unity.Entities;

namespace vikwhite.ECS
{
    public struct AbilityRuntimeData : IBufferElementData
    {
        public uint ID;
        public int Level;
        public BlobAssetReference<AbilityConfig> Config;
    }

    public static class AbilityRuntimeDataExtensions
    {
        public static BlobAssetReference<AbilityConfig> Get(this DynamicBuffer<AbilityRuntimeData> buffer, uint id, int level)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].ID == id && buffer[i].Level == level)
                {
                    return buffer[i].Config;
                }
            }

            return default;
        }
    }
}
