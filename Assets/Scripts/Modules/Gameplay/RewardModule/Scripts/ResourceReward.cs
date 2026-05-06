namespace vikwhite
{
    public class ResourceReward : Reward
    {
        public ResourceType ResourceType;

        public override bool IsSameAs(Reward other)
        {
            return other is ResourceReward resource && resource.ResourceType == ResourceType;
        }
    }
}
