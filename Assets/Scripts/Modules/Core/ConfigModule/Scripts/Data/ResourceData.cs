using System;

namespace vikwhite.Data
{
    public interface IResourceData
    {
        string ID { get; }
        ResourceType Type { get; }
        RarityType Rarity { get; }
    }
    
    [Serializable]
    public class ResourceData : IResourceData
    {
        public string ID;
        public ResourceType Type;
        public RarityType Rarity;
        
        string IResourceData.ID => ID;
        ResourceType IResourceData.Type => Type;
        RarityType IResourceData.Rarity => Rarity;
    }
}