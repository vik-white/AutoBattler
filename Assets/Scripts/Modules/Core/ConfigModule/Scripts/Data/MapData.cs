using System;
using System.Collections.Generic;
using UnityEngine;

namespace vikwhite.Data
{
    public interface IMapData
    {
        string ID { get; }
        LocationType Type { get; }
        string Sector { get; }
        GameObject Prefab { get; }
    }
    
    [Serializable]
    public class MapData : IMapData, ICustomJsonParser
    {
        public string ID;
        public LocationType Type;
        public string Sector;
        public GameObject Prefab;
        
        string IMapData.ID => ID;
        LocationType IMapData.Type => Type;
        string IMapData.Sector => Sector;
        GameObject IMapData.Prefab => Prefab;
        
        public void Parse(Dictionary<string, string> row)
        {
            var prefabName = row["Prefab"];
            if (prefabName != "") Prefab = Resources.Load<GameObject>($"Characters/Prefabs/{prefabName}/{prefabName}");
        }
    }
}
