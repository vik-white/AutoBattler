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
        uint AbilityID { get; }
        bool Skill { get; }
        AbilityType Type { get; }
        string Icon { get; }
        float Cooldown { get; }
        float Radius { get; }
        float AOE { get; }
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
        List<uint> Abilities { get; }
        float ImpulseUp { get; }
        float ImpulseProvider { get; }
        uint CastVFXPrefab { get; }
        uint VFXPrefab { get; }
        uint ProjectilePrefab { get; }
        VFXSpawnType VFXSpawn { get; }
        AnimationType Animation { get; }
        Sprite IconImage { get; }
        string Description { get; }
    }
    
    [Serializable]
    public class AbilityData : IAbilityData, ICustomJsonParser
    {
        public uint AbilityID;
        public bool Skill;
        public AbilityType Type;
        public string Icon;
        public float Cooldown;
        public float Radius;
        public float AOE;
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
        public List<uint> Abilities;
        public float ImpulseUp;
        public float ImpulseProvider;
        public uint CastVFXPrefab;
        public uint VFXPrefab;
        public uint ProjectilePrefab;
        public VFXSpawnType VFXSpawn;
        public AnimationType Animation;
        public Sprite IconImage;
        public string Description;
        
        uint IAbilityData.AbilityID => AbilityID;
        bool IAbilityData.Skill => Skill;
        AbilityType IAbilityData.Type => Type;
        string IAbilityData.Icon => Icon;
        float IAbilityData.Cooldown => Cooldown;
        float IAbilityData.Radius => Radius;
        float IAbilityData.AOE => AOE;
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
        List<uint> IAbilityData.Abilities => Abilities;
        float IAbilityData.ImpulseUp => ImpulseUp;
        float IAbilityData.ImpulseProvider => ImpulseProvider;
        uint IAbilityData.CastVFXPrefab => CastVFXPrefab;
        uint IAbilityData.VFXPrefab => VFXPrefab;
        uint IAbilityData.ProjectilePrefab => ProjectilePrefab;
        VFXSpawnType IAbilityData.VFXSpawn => VFXSpawn;
        AnimationType IAbilityData.Animation => Animation;
        Sprite IAbilityData.IconImage => IconImage;
        string IAbilityData.Description => Description;
        
        public void Parse(Dictionary<string, string> row)
        {
            IconImage = Resources.Load<Sprite>("Abilities/Icons/" + row["Icon"]);
            
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
                var subParts = parts[1].Split('-');
                var valueString = subParts.Length == 1 ? subParts[0] : subParts[1];
                var dependenceString = subParts.Length == 1 ? "" : subParts[0];
                var dependence = EffectDependenceType.None;
                if(Enum.TryParse<EffectDependenceType>(dependenceString, out var dependenceType)) dependence = dependenceType;
                
                if (!Enum.TryParse<EffectType>(typeString, out var type)) continue;
                
                Effects.Add(new EffectData { Type = type, Dependence = dependence, Value = valueString.ToFloat() });
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
                Abilities.Add(abilityString.CalculateHash32());
            }
        }
    }
}
