using System;

namespace vikwhite
{
    public interface IStarLockLevelData
    {
        int ID { get; }
        int Level { get; }
    }
    
    [Serializable]
    public class StarLockLevelData : IStarLockLevelData
    {
        public int ID;
        public int Level;
        
        int IStarLockLevelData.ID => ID;
        int IStarLockLevelData.Level => Level;
    }
}