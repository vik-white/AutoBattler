namespace vikwhite
{
    public class ClassBookReward : Reward
    {
        public CharacterClassType Class;

        public override bool IsSameAs(Reward other)
        {
            return other is ClassBookReward book && book.Class == Class;
        }
    }
}
