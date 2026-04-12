using System;

namespace vikwhite.Data
{
    public interface ISquadData
    {
        string CharacterID { get; }
    }
    
    [Serializable]
    public class SquadData : ISquadData
    {
        public string CharacterID;
        
        string ISquadData.CharacterID => CharacterID;
    }
}