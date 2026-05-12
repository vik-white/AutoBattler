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
        bool HealthBar { get; }
        string ActiveAbility { get; }
        string LevelUp { get; }
        string StarLevelUp { get; }
        string SkillLevelUp { get; }
        bool Squad { get; }
        Sprite Image { get; }
        Sprite PortraitImage { get; }
        List<uint> Abilities { get; }
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
        public bool HealthBar;
        public string ActiveAbility;
        public string LevelUp;
        public string StarLevelUp;
        public string SkillLevelUp;
        public bool Squad;
        public Sprite Image;
        public Sprite PortraitImage;
        public List<uint> Abilities;
        
        string ICharacterData.ID => ID;
        string ICharacterData.Name => Name;
        string ICharacterData.Prefab => Prefab;
        float ICharacterData.Scale => Scale;
        float ICharacterData.Health => Health;
        float ICharacterData.Shield => Shield;
        bool ICharacterData.HealthBar => HealthBar;
        string ICharacterData.ActiveAbility => ActiveAbility;
        string ICharacterData.LevelUp => LevelUp;
        string ICharacterData.StarLevelUp => StarLevelUp;
        string ICharacterData.SkillLevelUp => SkillLevelUp;
        bool ICharacterData.Squad => Squad;
        Sprite ICharacterData.Image => Image;
        Sprite ICharacterData.PortraitImage => PortraitImage;
        List<uint> ICharacterData.Abilities => Abilities;
        
        public void Parse(Dictionary<string, string> row)
        {
            Abilities = new ();
            foreach (var abilityString in row["Abilities"].Split(";"))
            {
                if(abilityString == "") continue;
                Abilities.Add(abilityString.CalculateHash32());
            }
            if(row["Image"] != "") Image = Resources.Load<Sprite>($"Characters/Images/{row["Image"]}");
            if(row["Image"] != "") PortraitImage = Resources.Load<Sprite>($"Characters/PortraitImages/{row["Image"]}");
        }
    }
}
