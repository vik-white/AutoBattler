using Unity.Entities;

namespace vikwhite.ECS
{
    public struct AbilityRuntimeData : IBufferElementData
    {
        public BlobAssetReference<AbilityConfig> Config;
    }

    public static class AbilityRuntimeDataExtensions
    {
        public static BlobAssetReference<AbilityConfig> Get(this DynamicBuffer<AbilityRuntimeData> buffer, uint id)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Config.Value.ID == id)
                {
                    return buffer[i].Config;
                }
            }

            return default;
        }
    }
}
