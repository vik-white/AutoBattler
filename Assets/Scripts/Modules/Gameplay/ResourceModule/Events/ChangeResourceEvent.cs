namespace vikwhite
{
    public class ChangeResourceEvent
    {
        public ResourceType Type;
        public int Amount;

        public ChangeResourceEvent(ResourceType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}