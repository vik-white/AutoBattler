using System;
using System.Collections.Generic;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite.Data
{
    public interface ICharacterData
    {
        string ID { get; }
        string Name { get; }
        string Prefab { get; }
        RarityType Rarity { get; }
        CharacterClassType Class { get; }
        float Scale { get; }
        float Health { get; }
        float Shield { get; }
        float Attack { get; }
        float Defense { get; }
        float CritChance { get; }
        float CritValue { get; }
        bool Squad { get; }
        bool HealthBar { get; }
        string LevelUpgrade { get; }
        string StarUpgrade { get; }
        string SkillUpgrade { get; }
        Sprite Image { get; }
        Sprite ShardImage { get; }
        GameObject HeadPrefab { get; }
        IReadOnlyDictionary<SkillSlotType, uint> Skills { get; }
        uint GetSkill(SkillSlotType slotType);
        float GetStat(StatType stat);
    }

    [Serializable]
    public class CharacterData : ICharacterData, ICustomJsonParser
    {
        public string ID;
        public string Name;
        public string Prefab;
        public RarityType Rarity;
        public CharacterClassType Class;
        public float Scale;
        public float Health;
        public float Shield;
        public float Attack;
        public float Defense;
        public float CritChance;
        public float CritValue;
        public bool Squad;
        public bool HealthBar;
        public string LevelUpgrade;
        public string StarUpgrade;
        public string SkillUpgrade;
        public Sprite Image;
        public Sprite ShardImage;
        public GameObject HeadPrefab;

        public uint SkillAttack;
        public uint SkillActive;
        public uint SkillPassive1;
        public uint SkillPassive2;
        public uint SkillMeta1;
        public uint SkillMeta2;
        public uint SkillMeta3;

        private Dictionary<SkillSlotType, uint> _skills;

        public IReadOnlyDictionary<SkillSlotType, uint> Skills => _skills ??= BuildSkills();

        string ICharacterData.ID => ID;
        string ICharacterData.Name => Name;
        string ICharacterData.Prefab => Prefab;
        RarityType ICharacterData.Rarity => Rarity;
        CharacterClassType ICharacterData.Class => Class;
        float ICharacterData.Scale => Scale;
        float ICharacterData.Health => Health;
        float ICharacterData.Shield => Shield;
        float ICharacterData.Attack => Attack;
        float ICharacterData.Defense => Defense;
        float ICharacterData.CritChance => CritChance;
        float ICharacterData.CritValue => CritValue;
        bool ICharacterData.Squad => Squad;
        bool ICharacterData.HealthBar => HealthBar;
        string ICharacterData.LevelUpgrade => LevelUpgrade;
        string ICharacterData.StarUpgrade => StarUpgrade;
        string ICharacterData.SkillUpgrade => SkillUpgrade;
        Sprite ICharacterData.Image => Image;
        Sprite ICharacterData.ShardImage => ShardImage;
        GameObject ICharacterData.HeadPrefab => HeadPrefab;
        IReadOnlyDictionary<SkillSlotType, uint> ICharacterData.Skills => Skills;

        public uint GetSkill(SkillSlotType slotType) => Skills.TryGetValue(slotType, out var id) ? id : 0;

        public float GetStat(StatType stat) => stat switch
        {
            StatType.Attack => Attack,
            StatType.Defense => Defense,
            StatType.Health => Health,
            StatType.CritChance => CritChance,
            StatType.CritValue => CritValue,
            _ => 0f,
        };

        private Dictionary<SkillSlotType, uint> BuildSkills() => new()
        {
            { SkillSlotType.Attack, SkillAttack },
            { SkillSlotType.Active, SkillActive },
            { SkillSlotType.Passive1, SkillPassive1 },
            { SkillSlotType.Passive2, SkillPassive2 },
            { SkillSlotType.Meta1, SkillMeta1 },
            { SkillSlotType.Meta2, SkillMeta2 },
            { SkillSlotType.Meta3, SkillMeta3 },
        };

        public void Parse(Dictionary<string, string> row)
        {
            if (row["Image"] != "") Image = Resources.Load<Sprite>($"Characters/Images/{row["Image"]}");
            if (row["Image"] != "") ShardImage = Resources.Load<Sprite>($"Characters/Shards/{row["Image"]}");
            if (row["Image"] != "") HeadPrefab = Resources.Load<GameObject>($"Characters/HeadPrefabs/{row["Image"]}");
            _skills = null;
        }
    }
}
