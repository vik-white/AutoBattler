namespace vikwhite
{
    public class ChangeClassShardEvent
    {
        public CharacterClassType Class;
        public RarityType Rarity;
        public int Amount;

        public ChangeClassShardEvent(CharacterClassType @class, RarityType rarity, int amount)
        {
            Class = @class;
            Rarity = rarity;
            Amount = amount;
        }
    }
}
