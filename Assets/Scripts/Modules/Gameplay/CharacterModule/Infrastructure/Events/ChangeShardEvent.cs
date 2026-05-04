namespace vikwhite
{
    public class ChangeShardEvent
    {
        public string ID;
        public int Count;

        public ChangeShardEvent(string id, int count)
        {
            ID = id;
            Count = count;
        }
    }
}
