using System;
using System.Collections.Generic;

namespace vikwhite
{
    public interface IStarData
    {
        int ID { get; }
        int Level { get; }
        IReadOnlyDictionary<SkillSlotType, int> SkillUnlocks { get; }
        int GetSkillUnlock(SkillSlotType slotType);
    }

    [Serializable]
    public class StarData : IStarData
    {
        public int ID;
        public int Level;

        public int SkillActive;
        public int SkillPassive1;
        public int SkillPassive2;
        public int SkillMeta1;
        public int SkillMeta2;
        public int SkillMeta3;

        private Dictionary<SkillSlotType, int> _skillUnlocks;

        public IReadOnlyDictionary<SkillSlotType, int> SkillUnlocks => _skillUnlocks ??= BuildSkillUnlocks();

        int IStarData.ID => ID;
        int IStarData.Level => Level;
        IReadOnlyDictionary<SkillSlotType, int> IStarData.SkillUnlocks => SkillUnlocks;

        public int GetSkillUnlock(SkillSlotType slotType) => SkillUnlocks.TryGetValue(slotType, out var value) ? value : 0;

        private Dictionary<SkillSlotType, int> BuildSkillUnlocks() => new()
        {
            { SkillSlotType.Active, SkillActive },
            { SkillSlotType.Passive1, SkillPassive1 },
            { SkillSlotType.Passive2, SkillPassive2 },
            { SkillSlotType.Meta1, SkillMeta1 },
            { SkillSlotType.Meta2, SkillMeta2 },
            { SkillSlotType.Meta3, SkillMeta3 },
        };
    }
}
