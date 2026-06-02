namespace vikwhite
{
    public interface IEventItemViewFactory : IPooledViewFactory<EventItemView, EventItemViewModel> { }

    public class EventItemViewFactory : PooledViewFactory<EventItemView, EventItemViewModel>, IEventItemViewFactory
    {
        public override string AssetName => "UI/Prefabs/Elements/EventItem";
    }
}
