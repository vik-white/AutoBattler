using System;

namespace vikwhite.Data
{
    public interface IMapData
    {
        string ID { get; }
        LocationType Type { get; }
        string Sector { get; }
    }
    
    [Serializable]
    public class MapData : IMapData
    {
        public string ID;
        public LocationType Type;
        public string Sector;
        
        string IMapData.ID => ID;
        LocationType IMapData.Type => Type;
        string IMapData.Sector => Sector;
    }
}
