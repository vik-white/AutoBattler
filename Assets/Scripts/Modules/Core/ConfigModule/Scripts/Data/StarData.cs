using System;
using System.Collections.Generic;

namespace vikwhite
{
    public interface IStarData
    {
        int ID { get; }
        int Level { get; }
        IReadOnlyDictionary<SkillType, int> SkillUnlocks { get; }
        int GetSkillUnlock(SkillType slot);
    }

    [Serializable]
    public class StarData : IStarData
    {
        public int ID;
        public int Level;

        // Stored per-slot for backwards compatibility with the existing asset and Google Sheet columns.
        // Code should access unlocks via the dictionary view below.
        public int SkillActive;
        public int SkillPassive1;
        public int SkillPassive2;
        public int SkillMeta1;
        public int SkillMeta2;
        public int SkillMeta3;

        private Dictionary<SkillType, int> _skillUnlocks;

        public IReadOnlyDictionary<SkillType, int> SkillUnlocks => _skillUnlocks ??= BuildSkillUnlocks();

        int IStarData.ID => ID;
        int IStarData.Level => Level;
        IReadOnlyDictionary<SkillType, int> IStarData.SkillUnlocks => SkillUnlocks;

        public int GetSkillUnlock(SkillType slot) => SkillUnlocks.TryGetValue(slot, out var value) ? value : 0;

        private Dictionary<SkillType, int> BuildSkillUnlocks() => new()
        {
            { SkillType.Active, SkillActive },
            { SkillType.Passive1, SkillPassive1 },
            { SkillType.Passive2, SkillPassive2 },
            { SkillType.Meta1, SkillMeta1 },
            { SkillType.Meta2, SkillMeta2 },
            { SkillType.Meta3, SkillMeta3 },
        };
    }
}
