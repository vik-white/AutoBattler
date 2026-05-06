namespace vikwhite
{
    public abstract class Reward
    {
        public int Value;

        public abstract bool IsSameAs(Reward other);
    }
}
