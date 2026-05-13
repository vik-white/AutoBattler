using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    public struct Skill : IBufferElementData
    {
        public BlobAssetReference<SkillConfig> Config;
        public float Cooldown;
        public bool IsActivate;
        public bool IsAnimation;
        public bool IsChild;
    }

    public static class AbilityExtensions
    {
        public static SkillConfig GetConfig(this in Skill skill)
        {
            return skill.Config.Value;
        }

        public static bool TryGetActivatedConfig(this in Skill skill, SkillType type, out SkillConfig config)
        {
            config = skill.Config.Value;
            return skill.IsActivate && config.Type == type;
        }
    }
}
