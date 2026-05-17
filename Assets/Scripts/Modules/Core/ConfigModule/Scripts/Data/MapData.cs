using System;

namespace vikwhite.Data
{
    public interface IMapData
    {
        string ID { get; }
        LocationType Type { get; }
        bool RoadMap { get; }
        string Sector { get; }
    }
    
    [Serializable]
    public class MapData : IMapData
    {
        public string ID;
        public LocationType Type;
        public bool RoadMap;
        public string Sector;
        
        string IMapData.ID => ID;
        LocationType IMapData.Type => Type;
        bool IMapData.RoadMap => RoadMap;
        string IMapData.Sector => Sector;
    }
}
