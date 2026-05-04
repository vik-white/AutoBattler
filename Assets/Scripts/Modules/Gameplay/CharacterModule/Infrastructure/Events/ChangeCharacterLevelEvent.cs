namespace vikwhite
{
    public class ChangeCharacterLevelEvent
    {
        public string ID;
        public int Level;

        public ChangeCharacterLevelEvent(string id, int level)
        {
            ID = id;
            Level = level;
        }
    }
}