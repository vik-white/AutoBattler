using System;

namespace vikwhite.Data
{
    public interface ISummonData
    {
        string ID { get; }
        ResourceType Resource { get; }
        string Reward { get; }
    }

    [Serializable]
    public class SummonData : ISummonData
    {
        public string ID;
        public ResourceType Resource;
        public string Reward;

        string ISummonData.ID => ID;
        ResourceType ISummonData.Resource => Resource;
        string ISummonData.Reward => Reward;
    }
}
