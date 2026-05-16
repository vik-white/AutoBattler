using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public struct Skill : IBufferElementData
    {
        public BlobAssetReference<SkillConfig> Config;
        public float Cooldown;
    }

    public static class SkillExtensions
    {
        public static SkillConfig GetConfig(this in Skill skill)
        {
            return skill.Config.Value;
        }
    }
}
