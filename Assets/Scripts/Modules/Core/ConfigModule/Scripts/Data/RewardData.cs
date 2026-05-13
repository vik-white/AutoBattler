using System;

namespace vikwhite
{
    [Serializable]
    public struct RewardData
    {
        public RewardType Type;
        public ResourceType ResourceType;
        public ShardGroupType ShardGroupType;
        public CharacterClassType Class;
        public RarityType Rarity;
        public string ID;
        public int MinValue;
        public int MaxValue;
        public float Probability;
    }
}