using System;
using System.Collections.Generic;
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
        
        string IAbilityData.AbilityID => AbilityID;
        int IAbilityData.Level => Level;
        AbilityType IAbilityData.Type => Type;
        string IAbilityData.Icon => Icon;
        float IAbilityData.Cooldown => Cooldown;
        float IAbilityData.Radius => Radius;
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
                if (!float.TryParse(valueString, out var value)) continue;
                
                Effects.Add(new EffectData { Type = type, Value = value });
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
                
                if (!Enum.TryParse<StatusType>(typeString, out var type)) continue;
                if (!float.TryParse(valueString, out var value)) continue;
                if (!float.TryParse(durationString, out var duration)) continue;
                if (!float.TryParse(periodString, out var period)) continue;
                
                Statuses.Add(new StatusData
                {
                    Type = type, 
                    Value = value,
                    Duration = duration,
                    Period = period,
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
                if (!float.TryParse(valueString, out var value)) continue;
                if (!float.TryParse(durationString, out var duration)) continue;
                
                Stats.Add(new StatData
                {
                    Type = type, 
                    Value = value,
                    Duration = duration,
                });
            }
        }
    }
}