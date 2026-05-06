using System;

namespace vikwhite.Data
{
    public interface ISummonData
    {
        string ID { get; }
        string Name { get; }
        ResourceType Resource { get; }
        int Price { get; }
        string Reward { get; }
    }

    [Serializable]
    public class SummonData : ISummonData
    {
        public string ID;
        public string Name;
        public ResourceType Resource;
        public int Price;
        public string Reward;

        string ISummonData.ID => ID;
        string ISummonData.Name => Name;
        ResourceType ISummonData.Resource => Resource;
        int ISummonData.Price => Price;
        string ISummonData.Reward => Reward;
    }
}
