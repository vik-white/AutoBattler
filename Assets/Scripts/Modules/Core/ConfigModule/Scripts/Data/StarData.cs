using System;
using System.Collections.Generic;

namespace vikwhite
{
    public interface IStarData
    {
        int ID { get; }
        int Level { get; }
        int GetMaxSkillLevel(SkillSlotType slotType);
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

        private Dictionary<SkillSlotType, int> _maxSkillLevels;

        public IReadOnlyDictionary<SkillSlotType, int> MaxSkillLevels => _maxSkillLevels ??= BuildMaxSkillLevels();

        int IStarData.ID => ID;
        int IStarData.Level => Level;

        public int GetMaxSkillLevel(SkillSlotType slotType) => MaxSkillLevels.TryGetValue(slotType, out var value) ? value : 0;

        private Dictionary<SkillSlotType, int> BuildMaxSkillLevels() => new()
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
