using System;
using System.Collections.Generic;

namespace vikwhite.Data
{
    public interface IRoomData
    {
        string Room { get; }
        int Level { get; }
        ResourceType Production { get; }
        List<ResourceCountData> ProductionUpgrade { get; }
        List<ResourceCountData> CapacityUpgrade { get; }
        List<ResourceCountData> ResRequirements { get; }
        List<RoomLevelData> RoomRequirements { get; }
        float UpgradeTime { get; }
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
        public string Room;
        public int Level;
    }
    
    [Serializable]
    public class RoomData: IRoomData, ICustomJsonParser
    {
        public string Room;
        public int Level;
        public ResourceType Production;
        public List<ResourceCountData> ProductionUpgrade = new();
        public List<ResourceCountData> CapacityUpgrade = new();
        public List<ResourceCountData> ResRequirements = new();
        public List<RoomLevelData> RoomRequirements = new();
        public float UpgradeTime;
        
        string IRoomData.Room => Room;
        int IRoomData.Level => Level;
        ResourceType IRoomData.Production => Production;
        List<ResourceCountData> IRoomData.ProductionUpgrade => ProductionUpgrade;
        List<ResourceCountData> IRoomData.CapacityUpgrade => CapacityUpgrade;
        List<ResourceCountData> IRoomData.ResRequirements => ResRequirements;
        List<RoomLevelData> IRoomData.RoomRequirements => RoomRequirements;
        float IRoomData.UpgradeTime => UpgradeTime;
        
        public void Parse(Dictionary<string, string> row)
        {
            foreach (var srt in row["ProductionUpgrade"].Split(";"))
            {
                if(srt == "") continue;
                var parts = srt.Split(':');
                var typeString = parts[0];
                var valueString = parts[1];
                if (!Enum.TryParse<ResourceType>(typeString, out var type)) continue;
                ProductionUpgrade.Add(new ResourceCountData { Resource = type, Count = valueString.ToFloat() });
            }
            
            foreach (var srt in row["CapacityUpgrade"].Split(";"))
            {
                if(srt == "") continue;
                var parts = srt.Split(':');
                var typeString = parts[0];
                var valueString = parts[1];
                if (!Enum.TryParse<ResourceType>(typeString, out var type)) continue;
                CapacityUpgrade.Add(new ResourceCountData { Resource = type, Count = valueString.ToFloat() });
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
                RoomRequirements.Add(new RoomLevelData { Room = typeString, Level = int.Parse(valueString) });
            }
        }
    }
}