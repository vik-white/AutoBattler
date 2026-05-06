using System;

namespace vikwhite
{
    public interface IStarLockData
    {
        int ID { get; }
        int Level { get; }
    }
    
    [Serializable]
    public class StarLockData : IStarLockData
    {
        public int ID;
        public int Level;
        
        int IStarLockData.ID => ID;
        int IStarLockData.Level => Level;
    }
}