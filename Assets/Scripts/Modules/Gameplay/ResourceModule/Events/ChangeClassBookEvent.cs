namespace vikwhite
{
    public class ChangeClassBookEvent
    {
        public CharacterClassType Class;
        public int Amount;

        public ChangeClassBookEvent(CharacterClassType @class, int amount)
        {
            Class = @class;
            Amount = amount;
        }
    }
}
