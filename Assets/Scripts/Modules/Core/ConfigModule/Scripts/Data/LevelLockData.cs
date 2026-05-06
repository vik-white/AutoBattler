using System;

namespace vikwhite
{
    public interface ILevelLockData
    {
        int ID { get; }
        int Skill { get; }
    }

    [Serializable]
    public class LevelLockData : ILevelLockData
    {
        public int ID;
        public int Skill;
        
        int ILevelLockData.ID => ID;
        int ILevelLockData.Skill => Skill;
    }
}