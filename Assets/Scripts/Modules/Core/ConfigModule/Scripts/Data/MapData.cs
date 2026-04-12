using System;

namespace vikwhite.Data
{
    public interface IMapData
    {
        string LocationID { get; }
        LocationType LocationType { get; }
    }
    
    [Serializable]
    public class MapData : IMapData
    {
        public string LocationID;
        public LocationType LocationType;
        
        string IMapData.LocationID => LocationID;
        LocationType IMapData.LocationType => LocationType;
    }
}