using System;
using System.Collections.Generic;
using Rukhanka.Toolbox;

namespace vikwhite.Data
{
    public interface ILocationStaticData
    {
        string ID { get; }
        string HexPositions { get; }
        string Reward { get; }
        List<CharacterLevelData> Enemies { get; }
    }
    
    [Serializable]
    public class LocationStaticData : ILocationStaticData, ICustomJsonParser
    {
        public string ID;
        public string HexPositions;
        public string Reward;
        public List<CharacterLevelData> Enemies;
        
        string ILocationStaticData.ID => ID;
        string ILocationStaticData.HexPositions => HexPositions;
        string ILocationStaticData.Reward => Reward;
        List<CharacterLevelData> ILocationStaticData.Enemies => Enemies;
        
        public void Parse(Dictionary<string, string> row)
        {
            Enemies = new ();
            foreach (var enemyString in row["Enemies"].Split(";"))
            {
                if(enemyString == "") continue;
                var parts = enemyString.Split(':');
                var idString = parts[0];
                var levelString = parts[1];
                
                if (!int.TryParse(levelString, out var level)) continue;
                Enemies.Add(new CharacterLevelData{ ID = idString.CalculateHash32(), Level = level });
            }
        }
    }
}