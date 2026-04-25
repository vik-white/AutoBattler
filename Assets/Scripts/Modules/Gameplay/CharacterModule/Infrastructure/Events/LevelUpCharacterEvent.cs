namespace vikwhite
{
    public class LevelUpCharacterEvent
    {
        public string ID;
        public int Level;

        public LevelUpCharacterEvent(string id, int level)
        {
            ID = id;
            Level = level;
        }
    }
}