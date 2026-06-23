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

        public static ResourceType GetBookResourceType(CharacterClassType characterClass)
            => characterClass switch
            {
                CharacterClassType.Assassin => ResourceType.BookAssassin,
                CharacterClassType.Mage => ResourceType.BookMage,
                CharacterClassType.Mystic => ResourceType.BookMystic,
                CharacterClassType.Support => ResourceType.BookSupport,
                _ => ResourceType.BookTank,
            };
    }
}
