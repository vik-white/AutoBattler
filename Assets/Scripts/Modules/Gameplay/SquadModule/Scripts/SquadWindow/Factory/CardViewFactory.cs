namespace vikwhite
{
    public interface ISquadItemViewFactory : IPooledViewFactory<SquadItemView, SquadItemViewModel> { }
    
    public class SquadItemViewFactory : PooledViewFactory<SquadItemView, SquadItemViewModel>, ISquadItemViewFactory
    {
        public override string AssetName => "UI/Prefabs/SquadWindow/SquadItem";
    }
}