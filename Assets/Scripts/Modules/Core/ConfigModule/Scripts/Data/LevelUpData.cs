using System;

namespace vikwhite.Data
{
    public interface ILevelUpData
    {
        string ID { get; }
        float Damage { get; }
        float Health { get; }
        float Shield { get; }
        float Heal { get; }
    }
    
    [Serializable]
    public class LevelUpData : ILevelUpData
    {
        public string ID;
        public float Damage;
        public float Health;
        public float Shield;
        public float Heal;
        
        string ILevelUpData.ID => ID;
        float ILevelUpData.Damage => Damage;
        float ILevelUpData.Health => Health;
        float ILevelUpData.Shield => Shield;
        float ILevelUpData.Heal => Heal;
    }
}