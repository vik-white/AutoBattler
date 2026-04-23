using System;

namespace vikwhite.Data
{
    public interface IMapData
    {
        string LocationID { get; }
        bool RoadMap { get; }
        LocationType LocationType { get; }
    }
    
    [Serializable]
    public class MapData : IMapData
    {
        public string LocationID;
        public bool RoadMap;
        public LocationType LocationType;
        
        string IMapData.LocationID => LocationID;
        bool IMapData.RoadMap => RoadMap;
        LocationType IMapData.LocationType => LocationType;
    }
}