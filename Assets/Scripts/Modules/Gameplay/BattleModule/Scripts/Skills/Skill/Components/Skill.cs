using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public struct Skill : IBufferElementData
    {
        public BlobAssetReference<SkillConfig> Config;
        public float Cooldown;
        public bool IsActivated;
        public bool IsAnimating;
        public bool IsChild;
    }

    public static class SkillExtensions
    {
        public static SkillConfig GetConfig(this in Skill skill)
        {
            return skill.Config.Value;
        }

        public static bool TryGetActivatedConfig(this in Skill skill, SkillType type, out SkillConfig config)
        {
            config = skill.Config.Value;
            return skill.IsActivated && config.Type == type;
        }
    }
}
