namespace vikwhite
{
    public class ChangeResourceEvent
    {
        public ResourceType Type;
        public int Amount;
        public int Delta;

        public ChangeResourceEvent(ResourceType type, int amount, int delta)
        {
            Type = type;
            Amount = amount;
            Delta = delta;
        }
    }
}
