using System;
using System.Collections.Generic;

namespace vikwhite.Data
{
    public interface ILocationFlowData
    {
        string LocationID { get; }
        float Time { get; }
        List<string> Enemies { get; }
        int Count { get; }
        float SpawnInterval { get; }
    }
    
    [Serializable]
    public class LocationFlowData : ILocationFlowData, ICustomJsonParser
    {
        public string LocationID;
        public float Time;
        public List<string> Enemies;
        public int Count;
        public float SpawnInterval;
        
        string ILocationFlowData.LocationID => LocationID;
        float ILocationFlowData.Time => Time;
        List<string> ILocationFlowData.Enemies => Enemies;
        int ILocationFlowData.Count => Count;
        float ILocationFlowData.SpawnInterval => SpawnInterval;
        
        public void Parse(Dictionary<string, string> row)
        {
            Enemies = new ();
            foreach (var enemyString in row["Enemies"].Split(";"))
                Enemies.Add(enemyString);
        }
    }
}