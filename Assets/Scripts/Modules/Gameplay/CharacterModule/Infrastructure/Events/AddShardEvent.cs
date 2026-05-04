namespace vikwhite
{
    public class AddShardEvent
    {
        public string ID;
        public int Amount;

        public AddShardEvent(string id, int amount)
        {
            ID = id;
            Amount = amount;
        }
    }
}
