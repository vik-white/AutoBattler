using Unity.Entities;

namespace vikwhite.ECS
{
    public struct SkillTriggerRequest
    {
        public readonly Entity Source;
        public readonly TriggerType Trigger;
        public readonly uint SkillID;
        public readonly bool IgnoreRadiusForSource;
        public readonly bool AllowDeadSourceOwner;

        public SkillTriggerRequest(
            Entity source,
            TriggerType trigger,
            uint skillID = 0,
            bool ignoreRadiusForSource = false,
            bool allowDeadSourceOwner = false)
        {
            Source = source;
            Trigger = trigger;
            SkillID = skillID;
            IgnoreRadiusForSource = ignoreRadiusForSource;
            AllowDeadSourceOwner = allowDeadSourceOwner;
        }

        public uint GetRequestedSkillID(Entity owner)
        {
            return owner == Source ? SkillID : 0;
        }

        public bool ShouldIgnoreRadius(Entity owner)
        {
            return IgnoreRadiusForSource && owner == Source;
        }
    }
}