using System;
using System.Collections.Generic;
using Rukhanka.Toolbox;
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
        string LevelUp { get; }
        string StarLevelUp { get; }
        string SkillLevelUp { get; }
        bool Squad { get; }
        Sprite Image { get; }
        Sprite PortraitImage { get; }
        uint Ability { get; }
        uint SkillActive { get; }
        uint SkillPassive1 { get; }
        uint SkillPassive2 { get; }
        uint SkillMeta1 { get; }
        uint SkillMeta2 { get; }
        uint SkillMeta3 { get; }
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
        public string LevelUp;
        public string StarLevelUp;
        public string SkillLevelUp;
        public bool Squad;
        public Sprite Image;
        public Sprite PortraitImage;
        public uint Ability;
        public uint SkillActive;
        public uint SkillPassive1;
        public uint SkillPassive2;
        public uint SkillMeta1;
        public uint SkillMeta2;
        public uint SkillMeta3;
        
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
        string ICharacterData.LevelUp => LevelUp;
        string ICharacterData.StarLevelUp => StarLevelUp;
        string ICharacterData.SkillLevelUp => SkillLevelUp;
        bool ICharacterData.Squad => Squad;
        Sprite ICharacterData.Image => Image;
        Sprite ICharacterData.PortraitImage => PortraitImage;
        uint ICharacterData.Ability => Ability;
        uint ICharacterData.SkillActive => SkillActive;
        uint ICharacterData.SkillPassive1 => SkillPassive1;
        uint ICharacterData.SkillPassive2 => SkillPassive2;
        uint ICharacterData.SkillMeta1 => SkillMeta1;
        uint ICharacterData.SkillMeta2 => SkillMeta2;
        uint ICharacterData.SkillMeta3 => SkillMeta3;
        
        public void Parse(Dictionary<string, string> row)
        {
            if(row["Image"] != "") Image = Resources.Load<Sprite>($"Characters/Images/{row["Image"]}");
            if(row["Image"] != "") PortraitImage = Resources.Load<Sprite>($"Characters/PortraitImages/{row["Image"]}");
        }
    }
}
