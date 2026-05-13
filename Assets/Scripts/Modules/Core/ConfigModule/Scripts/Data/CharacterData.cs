using System;
using System.Collections.Generic;
using UnityEngine;

namespace vikwhite.Data
{
    public interface ICharacterData
    {
        string ID { get; }
        string Name { get; }
        string Prefab { get; }
        float Scale { get; }
        float Health { get; }
        float Shield { get; }
        float Attack { get; }
        float Defense { get; }
        float CritChance { get; }
        float CritValue { get; }
        bool HealthBar { get; }
        string LevelUpgrade { get; }
        string StarUpgrade { get; }
        string SkillUpgrade { get; }
        bool Squad { get; }
        Sprite Image { get; }
        Sprite PortraitImage { get; }
        IReadOnlyDictionary<SkillType, uint> Skills { get; }
        uint GetSkill(SkillType slot);
    }

    [Serializable]
    public class CharacterData : ICharacterData, ICustomJsonParser
    {
        public string ID;
        public string Name;
        public string Prefab;
        public float Scale;
        public float Health;
        public float Shield;
        public float Attack;
        public float Defense;
        public float CritChance;
        public float CritValue;
        public bool HealthBar;
        public string LevelUpgrade;
        public string StarUpgrade;
        public string SkillUpgrade;
        public bool Squad;
        public Sprite Image;
        public Sprite PortraitImage;

        // Stored per-slot for backwards compatibility with the existing asset and Google Sheet columns.
        // Code should access skills via the dictionary view below.
        public uint SkillAttack;
        public uint SkillActive;
        public uint SkillPassive1;
        public uint SkillPassive2;
        public uint SkillMeta1;
        public uint SkillMeta2;
        public uint SkillMeta3;

        private Dictionary<SkillType, uint> _skills;

        public IReadOnlyDictionary<SkillType, uint> Skills => _skills ??= BuildSkills();

        string ICharacterData.ID => ID;
        string ICharacterData.Name => Name;
        string ICharacterData.Prefab => Prefab;
        float ICharacterData.Scale => Scale;
        float ICharacterData.Health => Health;
        float ICharacterData.Shield => Shield;
        float ICharacterData.Attack => Attack;
        float ICharacterData.Defense => Defense;
        float ICharacterData.CritChance => CritChance;
        float ICharacterData.CritValue => CritValue;
        bool ICharacterData.HealthBar => HealthBar;
        string ICharacterData.LevelUpgrade => LevelUpgrade;
        string ICharacterData.StarUpgrade => StarUpgrade;
        string ICharacterData.SkillUpgrade => SkillUpgrade;
        bool ICharacterData.Squad => Squad;
        Sprite ICharacterData.Image => Image;
        Sprite ICharacterData.PortraitImage => PortraitImage;
        IReadOnlyDictionary<SkillType, uint> ICharacterData.Skills => Skills;

        public uint GetSkill(SkillType slot) => Skills.TryGetValue(slot, out var id) ? id : 0;

        private Dictionary<SkillType, uint> BuildSkills() => new()
        {
            { SkillType.Attack, SkillAttack },
            { SkillType.Active, SkillActive },
            { SkillType.Passive1, SkillPassive1 },
            { SkillType.Passive2, SkillPassive2 },
            { SkillType.Meta1, SkillMeta1 },
            { SkillType.Meta2, SkillMeta2 },
            { SkillType.Meta3, SkillMeta3 },
        };

        public void Parse(Dictionary<string, string> row)
        {
            if (row["Image"] != "") Image = Resources.Load<Sprite>($"Characters/Images/{row["Image"]}");
            if (row["Image"] != "") PortraitImage = Resources.Load<Sprite>($"Characters/PortraitImages/{row["Image"]}");
            _skills = null;
        }
    }
}
