using System;

namespace vikwhite.Data
{
    public interface ICharacterData
    {
        string ID { get; }
        int Health { get; }
    }
    
    [Serializable]
    public class CharacterData : ICharacterData
    {
        public string ID;
        public int Health;
        
        string ICharacterData.ID => ID;
        int ICharacterData.Health => Health;
    }
}