using System;

namespace vikwhite.Data
{
    public interface ISquadData
    {
        string ID { get; }
    }
    
    [Serializable]
    public class SquadData : ISquadData
    {
        public string ID;
        
        string ISquadData.ID => ID;
    }
}