using System;
using System.Collections.Generic;
using System.Globalization;
using Rukhanka.Toolbox;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite.Data
{
    public interface IAbilityData
    {
        string AbilityID { get; }
        int Level { get; }
        AbilityType Type { get; }
        string Icon { get; }
        float Cooldown { get; }
        float Radius { get; }
        float AOE { get; }
        string Prefab { get; }
        List<TargetType> Targets { get; }
        List<StatData> Stats { get; }
        List<EffectData> Effects { get; }
        List<StatusData> Statuses { get; }
        int Count { get; }
        float Speed { get; }
        int Pierce { get; }
        float Scale { get; }
        float OrbitRadius { get; }
        float Lifetime { get; }
        List<SpawnCharacterData> SpawnCharacters { get; }
        float SpawnRadius { get; }
        float AuraLifetime { get; }
        float AuraRadius { get; }
        float AuraInterval { get; }
        List<AbilityLevelData> Abilities { get; }
    }
    
    [Serializable]
    public class AbilityData : IAbilityData, ICustomJsonParser
    {
        public string AbilityID;
        public int Level;
        public AbilityType Type;
        public string Icon;
        public float Cooldown;
        public float Radius;
        public float AOE;
        public string Prefab;
        public List<TargetType> Targets;
        public List<StatData> Stats;
        public List<EffectData> Effects;
        public List<StatusData> Statuses;
        public int Count;
        public float Speed;
        public int Pierce;
        public float Scale;
        public float OrbitRadius;
        public float Lifetime;
        public List<SpawnCharacterData> SpawnCharacters;
        public float SpawnRadius;
        public float AuraLifetime;
        public float AuraRadius;
        public float AuraInterval;
        public List<AbilityLevelData> Abilities;
        
        string IAbilityData.AbilityID => AbilityID;
        int IAbilityData.Level => Level;
        AbilityType IAbilityData.Type => Type;
        string IAbilityData.Icon => Icon;
        float IAbilityData.Cooldown => Cooldown;
        float IAbilityData.Radius => Radius;
        float IAbilityData.AOE => AOE;
        string IAbilityData.Prefab => Prefab;
        List<TargetType> IAbilityData.Targets => Targets;
        List<StatData> IAbilityData.Stats => Stats;
        List<EffectData> IAbilityData.Effects => Effects;
        List<StatusData> IAbilityData.Statuses => Statuses;
        int IAbilityData.Count => Count;
        float IAbilityData.Speed => Speed;
        int IAbilityData.Pierce => Pierce;
        float IAbilityData.Scale => Scale;
        float IAbilityData.OrbitRadius => OrbitRadius;
        float IAbilityData.Lifetime => Lifetime;
        List<SpawnCharacterData> IAbilityData.SpawnCharacters => SpawnCharacters;
        float IAbilityData.SpawnRadius => SpawnRadius;
        float IAbilityData.AuraLifetime => AuraLifetime;
        float IAbilityData.AuraRadius => AuraRadius;
        float IAbilityData.AuraInterval => AuraInterval;
        List<AbilityLevelData> IAbilityData.Abilities => Abilities;
        
        public void Parse(Dictionary<string, string> row)
        {
            Targets = new ();
            foreach (var abilityString in row["Targets"].Split(";"))
            {
                if (!Enum.TryParse<TargetType>(abilityString, out var type)) continue;
                Targets.Add(type);
            }
            
            Effects = new ();
            foreach (var abilityString in row["Effects"].Split(";"))
            {
                if(abilityString == "") continue;
                var parts = abilityString.Split(':');
                var typeString = parts[0];
                var valueString = parts[1];
                
                if (!Enum.TryParse<EffectType>(typeString, out var type)) continue;
                
                Effects.Add(new EffectData { Type = type, Value = valueString.ToFloat() });
            }
            
            Statuses = new ();
            foreach (var abilityString in row["Statuses"].Split(";"))
            {
                if(abilityString == "") continue;
                var parts = abilityString.Split(':');
                var typeString = parts[0];
                var valueString = parts[1];
                var durationString = parts[2];
                var periodString = parts[3];
                
                if (!Enum.TryParse<EffectType>(typeString, out var type)) continue;
                
                Statuses.Add(new StatusData
                {
                    Type = type, 
                    Value = valueString.ToFloat(),
                    Duration = durationString.ToFloat(),
                    Period = periodString.ToFloat(),
                });
            }
            
            Stats = new ();
            foreach (var abilityString in row["Stats"].Split(";"))
            {
                if(abilityString == "") continue;
                var parts = abilityString.Split(':');
                var typeString = parts[0];
                var valueString = parts[1];
                var durationString = parts[2];
                
                if (!Enum.TryParse<StatType>(typeString, out var type)) continue;
                
                Stats.Add(new StatData
                {
                    Type = type, 
                    Value = valueString.ToFloat(),
                    Duration = durationString.ToFloat(),
                });
            }
            
            SpawnCharacters = new ();
            foreach (var abilityString in row["SpawnCharacters"].Split(";"))
            {
                if(abilityString == "") continue;
                var parts = abilityString.Split(':');
                var typeString = parts[0];
                var valueString = parts[1];
                
                SpawnCharacters.Add(new SpawnCharacterData { ID = typeString.CalculateHash32(), Probability = valueString.ToFloat() });
            }
            
            Abilities = new ();
            foreach (var abilityString in row["Abilities"].Split(";"))
            {
                if(abilityString == "") continue;
                var parts = abilityString.Split(':');
                var idString = parts[0];
                var valueString = parts[1];
                
                if (!int.TryParse(valueString, out var value)) continue;
                
                Abilities.Add(new AbilityLevelData { ID = idString.CalculateHash32(), Level = value });
            }
        }
    }
}