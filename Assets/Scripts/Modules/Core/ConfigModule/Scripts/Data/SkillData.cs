using System;
using System.Collections.Generic;
using System.Globalization;
using Rukhanka.Toolbox;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite.Data
{
    public interface ISkillData
    {
        string ID { get; }
        string Name { get; }
        SkillType Type { get; }
        string Icon { get; }
        float Cooldown { get; }
        float Chance { get; }
        float Radius { get; }
        float AOE { get; }
        TriggerType Trigger { get; }
        TargetType TriggerSource { get; }
        List<TargetType> Targets { get; }
        int TargetsCount { get; }
        List<TargetConditionType> TargetConditions { get; }
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
        List<uint> Skills { get; }
        float ImpulseUp { get; }
        float ImpulseProvider { get; }
        uint CastVFXPrefab { get; }
        uint VFXPrefab { get; }
        uint ProjectilePrefab { get; }
        VFXSpawnType VFXSpawn { get; }
        AnimationType Animation { get; }
        Sprite IconImage { get; }
        string Description { get; }
        float Might { get; }
        float LevelMightMultiplier { get; }
    }
    
    [Serializable]
    public class SkillData : ISkillData, ICustomJsonParser
    {
        public string ID;
        public string Name;
        public SkillType Type;
        public string Icon;
        public float Cooldown;
        public float Chance;
        public float Radius;
        public float AOE;
        public TriggerType Trigger;
        public TargetType TriggerSource;
        public List<TargetType> Targets;
        public int TargetsCount;
        public List<TargetConditionType> TargetConditions = new();
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
        public List<uint> Skills;
        public float ImpulseUp;
        public float ImpulseProvider;
        public uint CastVFXPrefab;
        public uint VFXPrefab;
        public uint ProjectilePrefab;
        public VFXSpawnType VFXSpawn;
        public AnimationType Animation;
        public Sprite IconImage;
        public string Description;
        public float Might;
        public float LevelMightMultiplier;
        
        string ISkillData.ID => ID;
        string ISkillData.Name => Name;
        SkillType ISkillData.Type => Type;
        string ISkillData.Icon => Icon;
        float ISkillData.Cooldown => Cooldown;
        float ISkillData.Chance => Chance;
        float ISkillData.Radius => Radius;
        float ISkillData.AOE => AOE;
        TriggerType ISkillData.Trigger => Trigger;
        TargetType ISkillData.TriggerSource => TriggerSource;
        List<TargetType> ISkillData.Targets => Targets;
        int ISkillData.TargetsCount => TargetsCount;
        List<TargetConditionType> ISkillData.TargetConditions => TargetConditions;
        List<StatData> ISkillData.Stats => Stats;
        List<EffectData> ISkillData.Effects => Effects;
        List<StatusData> ISkillData.Statuses => Statuses;
        int ISkillData.Count => Count;
        float ISkillData.Speed => Speed;
        int ISkillData.Pierce => Pierce;
        float ISkillData.Scale => Scale;
        float ISkillData.OrbitRadius => OrbitRadius;
        float ISkillData.Lifetime => Lifetime;
        List<SpawnCharacterData> ISkillData.SpawnCharacters => SpawnCharacters;
        float ISkillData.SpawnRadius => SpawnRadius;
        float ISkillData.AuraLifetime => AuraLifetime;
        float ISkillData.AuraRadius => AuraRadius;
        float ISkillData.AuraInterval => AuraInterval;
        List<uint> ISkillData.Skills => Skills;
        float ISkillData.ImpulseUp => ImpulseUp;
        float ISkillData.ImpulseProvider => ImpulseProvider;
        uint ISkillData.CastVFXPrefab => CastVFXPrefab;
        uint ISkillData.VFXPrefab => VFXPrefab;
        uint ISkillData.ProjectilePrefab => ProjectilePrefab;
        VFXSpawnType ISkillData.VFXSpawn => VFXSpawn;
        AnimationType ISkillData.Animation => Animation;
        Sprite ISkillData.IconImage => IconImage;
        string ISkillData.Description => Description;
        float ISkillData.Might => Might;
        float ISkillData.LevelMightMultiplier => LevelMightMultiplier;
        
        public void Parse(Dictionary<string, string> row)
        {
            IconImage = Resources.Load<Sprite>("Abilities/Icons/" + row["Icon"]);
            
            Targets = new ();
            foreach (var abilityString in row["Targets"].Split(";"))
            {
                if (!Enum.TryParse<TargetType>(abilityString, out var type)) continue;
                Targets.Add(type);
            }

            TargetConditions = new ();
            foreach (var conditionString in row["TargetConditions"].Split(";"))
            {
                if (!Enum.TryParse<TargetConditionType>(conditionString, out var type)) continue;
                TargetConditions.Add(type);
            }
            
            Effects = new ();
            foreach (var abilityString in row["Effects"].Split(";"))
            {
                if(abilityString == "") continue;
                var parts = abilityString.Split(':');
                var typeString = parts[0];
                ParseEffectValue(parts[1], out var stat, out var value);
                
                if (!Enum.TryParse<EffectType>(typeString, out var type)) continue;
                
                Effects.Add(new EffectData { Type = type, Stat = stat, Value = value });
            }
            
            Statuses = new ();
            foreach (var abilityString in row["Statuses"].Split(";"))
            {
                if(abilityString == "") continue;
                var parts = abilityString.Split(':');
                var typeString = parts[0];
                var durationString = parts[2];
                var periodString = parts[3];
                ParseEffectValue(parts[1], out var stat, out var value);
                
                if (!Enum.TryParse<EffectType>(typeString, out var type)) continue;
                
                Statuses.Add(new StatusData
                {
                    Type = type,
                    Stat = stat,
                    UseStat = stat != StatType.None,
                    Value = value,
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
            
            Skills = new ();
            foreach (var abilityString in row["Skills"].Split(";"))
            {
                if(abilityString == "") continue;
                Skills.Add(abilityString.CalculateHash32());
            }
        }

        private static void ParseEffectValue(string rawValue, out StatType stat, out float value)
        {
            var parts = rawValue.Split('-');
            var valueString = parts.Length == 1 ? parts[0] : parts[1];
            var statString = parts.Length == 1 ? "" : parts[0];

            stat = StatType.None;
            if (!string.IsNullOrEmpty(statString) && !Enum.TryParse(statString, out stat))
                stat = StatType.None;

            value = valueString.ToFloat();
        }
    }
}
