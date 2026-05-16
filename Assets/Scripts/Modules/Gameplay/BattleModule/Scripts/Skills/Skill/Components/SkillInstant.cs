using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct SkillInstant : IBufferElementData
    {
        public Entity Trigger;
        public BlobAssetReference<SkillConfig> Skill;
        public FixedList128Bytes<BlobAssetReference<SkillConfig>> InheritedSkills;
    }
}
