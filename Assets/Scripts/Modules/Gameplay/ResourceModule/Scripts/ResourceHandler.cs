namespace vikwhite
{
    public class ResourceHandler
    {
        public static ResourceType GetShardResourceType(RarityType rarity)
            => rarity switch
            {
                RarityType.Epic => ResourceType.ShardEpic,
                RarityType.Legendary => ResourceType.ShardLegendary,
                _ => ResourceType.ShardRare
            };
    }
}