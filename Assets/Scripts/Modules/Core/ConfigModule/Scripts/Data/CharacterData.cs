using System;

namespace vikwhite.Data
{
    public interface ICharacterData
    {
        string ID { get; }
        string Prefab { get; }
        float Scale { get; }
        int Health { get; }
    }
    
    [Serializable]
    public class CharacterData : ICharacterData
    {
        public string ID;
        public string Prefab;
        public float Scale;
        public int Health;
        
        string ICharacterData.ID => ID;
        string ICharacterData.Prefab => Prefab;
        float ICharacterData.Scale => Scale;
        int ICharacterData.Health => Health;
    }
}