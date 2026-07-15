using System;
using System.Collections.Generic;
using UnityEngine;

namespace vikwhite.Data
{
    public interface IRoomData
    {
        RoomType Type { get; }
        GameObject Prefab { get; }
        int Level { get; }
        ResourceType Production { get; }
        List<RoomCountData> ProductionUpgrade { get; }
        List<RoomCountData> CapacityUpgrade { get; }
        List<ResourceCountData> ResRequirements { get; }
        List<RoomLevelData> RoomRequirements { get; }
        float UpgradeTime { get; }
    }
    
    [Serializable]
    public class RoomCountData
    {
        public RoomType Type;
        public float Count;
    }

    [Serializable]
    public class ResourceCountData
    {
        public ResourceType Resource;
        public float Count;
    }
    
    [Serializable]
    public class RoomLevelData
    {
        public RoomType Type;
        public int Level;
    }
    
    [Serializable]
    public class RoomData: IRoomData, ICustomJsonParser
    {
        public RoomType Type;
        public GameObject Prefab;
        public int Level;
        public ResourceType Production;
        public List<RoomCountData> ProductionUpgrade = new();
        public List<RoomCountData> CapacityUpgrade = new();
        public List<ResourceCountData> ResRequirements = new();
        public List<RoomLevelData> RoomRequirements = new();
        public float UpgradeTime;
        
        RoomType IRoomData.Type => Type;
        GameObject IRoomData.Prefab => Prefab;
        int IRoomData.Level => Level;
        ResourceType IRoomData.Production => Production;
        List<RoomCountData> IRoomData.ProductionUpgrade => ProductionUpgrade;
        List<RoomCountData> IRoomData.CapacityUpgrade => CapacityUpgrade;
        List<ResourceCountData> IRoomData.ResRequirements => ResRequirements;
        List<RoomLevelData> IRoomData.RoomRequirements => RoomRequirements;
        float IRoomData.UpgradeTime => UpgradeTime;
        
        public void Parse(Dictionary<string, string> row)
        {
            if (row["Prefab"] != "") Prefab = Resources.Load<GameObject>($"Rooms/{row["Prefab"]}");
            
            foreach (var srt in row["ProductionUpgrade"].Split(";"))
            {
                if(srt == "") continue;
                var parts = srt.Split(':');
                var typeString = parts[0];
                var valueString = parts[1];
                if (!Enum.TryParse<RoomType>(typeString, out var type)) continue;
                ProductionUpgrade.Add(new RoomCountData { Type = type, Count = valueString.ToFloat() });
            }
            
            foreach (var srt in row["CapacityUpgrade"].Split(";"))
            {
                if(srt == "") continue;
                var parts = srt.Split(':');
                var typeString = parts[0];
                var valueString = parts[1];
                if (!Enum.TryParse<RoomType>(typeString, out var type)) continue;
                CapacityUpgrade.Add(new RoomCountData { Type = type, Count = valueString.ToFloat() });
            }
            
            foreach (var srt in row["ResRequirements"].Split(";"))
            {
                if(srt == "") continue;
                var parts = srt.Split(':');
                var typeString = parts[0];
                var valueString = parts[1];
                if (!Enum.TryParse<ResourceType>(typeString, out var type)) continue;
                ResRequirements.Add(new ResourceCountData { Resource = type, Count = valueString.ToFloat() });
            }
            
            foreach (var srt in row["RoomRequirements"].Split(";"))
            {
                if(srt == "") continue;
                var parts = srt.Split(':');
                var typeString = parts[0];
                var valueString = parts[1];
                if (!Enum.TryParse<RoomType>(typeString, out var type)) continue;
                RoomRequirements.Add(new RoomLevelData { Type = type, Level = int.Parse(valueString) });
            }
        }
    }
}