namespace vikwhite
{
    public class ChangeCharacterShardEvent
    {
        public string ID;
        public int Count;

        public ChangeCharacterShardEvent(string id, int count)
        {
            ID = id;
            Count = count;
        }
    }
}
