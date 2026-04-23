using System;
using System.Collections.Generic;

namespace vikwhite.Data
{
    public interface ILocationStaticData
    {
        string ID { get; }
        int Rewards { get; }
        List<string> Enemies { get; }
    }
    
    [Serializable]
    public class LocationStaticData : ILocationStaticData, ICustomJsonParser
    {
        public string ID;
        public int Rewards;
        public List<string> Enemies;
        
        string ILocationStaticData.ID => ID;
        int ILocationStaticData.Rewards => Rewards;
        List<string> ILocationStaticData.Enemies => Enemies;
        
        public void Parse(Dictionary<string, string> row)
        {
            Enemies = new ();
            foreach (var enemyString in row["Enemies"].Split(";"))
                Enemies.Add(enemyString);
        }
    }
}