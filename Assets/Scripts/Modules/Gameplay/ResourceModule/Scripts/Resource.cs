using UniRx;

namespace vikwhite
{
    public class Resource
    {
        public ResourceType Type;
        public ReactiveProperty<int> Amount;

        public Resource(ResourceType type, int amount)
        {
            Type = type;
            Amount = new ReactiveProperty<int>(amount);
        }
    }
}