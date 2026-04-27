using System;
using System.Collections.Generic;
using Rukhanka.Toolbox;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite.Data
{
    public interface ICharacterData
    {
        string ID { get; }
        string Prefab { get; }
        float Scale { get; }
        float Mass { get; }
        float Health { get; }
        float Shield { get; }
        bool HealthBar { get; }
        string ActiveAbility { get; }
        string LevelUp { get; }
        bool Squad { get; }
        Sprite Image { get; }
        List<AbilityLevelData> Abilities { get; }
    }

    [Serializable]
    public class CharacterData : ICharacterData, ICustomJsonParser
    {
        public string ID;
        public string Prefab;
        public float Scale;
        public float Mass;
        public float Health;
        public float Shield;
        public bool HealthBar;
        public string ActiveAbility;
        public string LevelUp;
        public bool Squad;
        public Sprite Image;
        public List<AbilityLevelData> Abilities;
        
        string ICharacterData.ID => ID;
        string ICharacterData.Prefab => Prefab;
        float ICharacterData.Scale => Scale;
        float ICharacterData.Mass => Mass;
        float ICharacterData.Health => Health;
        float ICharacterData.Shield => Shield;
        bool ICharacterData.HealthBar => HealthBar;
        string ICharacterData.ActiveAbility => ActiveAbility;
        string ICharacterData.LevelUp => LevelUp;
        bool ICharacterData.Squad => Squad;
        Sprite ICharacterData.Image => Image;
        List<AbilityLevelData> ICharacterData.Abilities => Abilities;
        
        public void Parse(Dictionary<string, string> row)
        {
            Abilities = new ();
            foreach (var abilityString in row["Abilities"].Split(";"))
            {
                if(abilityString == "") continue;
                var parts = abilityString.Split(':');
                var idString = parts[0];
                var levelString = parts[1];
                
                if (!int.TryParse(levelString, out var level)) continue;
                
                Abilities.Add(new AbilityLevelData { ID = idString.CalculateHash32(), Level = level });
            }
            if(row["Image"] != "") Image = Resources.Load<Sprite>($"Characters/Images/{row["Image"]}");
        }
    }
}