using Unity.Entities;

namespace vikwhite.ECS
{
    public struct SkillRuntimeData : IBufferElementData
    {
        public BlobAssetReference<SkillConfig> Config;
    }

    public static class SkillRuntimeDataExtensions
    {
        public static BlobAssetReference<SkillConfig> Get(this DynamicBuffer<SkillRuntimeData> buffer, uint id)
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
