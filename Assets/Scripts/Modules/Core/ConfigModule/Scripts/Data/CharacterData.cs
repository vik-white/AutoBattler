using System;
using System.Collections.Generic;
using Rukhanka.Toolbox;
using vikwhite.ECS;

namespace vikwhite.Data
{
    public interface ICharacterData
    {
        string ID { get; }
        string Prefab { get; }
        float Scale { get; }
        float Mass { get; }
        int Health { get; }
        bool HealthBar { get; }
        string ActiveAbility { get; }
        List<AbilityLevelData> Abilities { get; }
    }
    
    [Serializable]
    public class CharacterData : ICharacterData, ICustomJsonParser
    {
        public string ID;
        public string Prefab;
        public float Scale;
        public float Mass;
        public int Health;
        public bool HealthBar;
        public string ActiveAbility;
        public List<AbilityLevelData> Abilities;
        
        string ICharacterData.ID => ID;
        string ICharacterData.Prefab => Prefab;
        float ICharacterData.Scale => Scale;
        float ICharacterData.Mass => Mass;
        int ICharacterData.Health => Health;
        bool ICharacterData.HealthBar => HealthBar;
        string ICharacterData.ActiveAbility => ActiveAbility;
        List<AbilityLevelData> ICharacterData.Abilities => Abilities;
        
        public void Parse(Dictionary<string, string> row)
        {
            Abilities = new ();
            foreach (var abilityString in row["Abilities"].Split(";"))
            {
                var parts = abilityString.Split(':');
                var idString = parts[0];
                var levelString = parts[1];
                
                if (!int.TryParse(levelString, out var level)) continue;
                
                Abilities.Add(new AbilityLevelData { ID = idString.CalculateHash32(), Level = level });
            }
        }
    }
}