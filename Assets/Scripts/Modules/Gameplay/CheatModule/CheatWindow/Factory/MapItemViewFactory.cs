namespace vikwhite
{
    public interface IMapItemViewFactory : IPooledViewFactory<MapItemView, MapItemViewModel> { }
    
    public class MapItemViewFactory : PooledViewFactory<MapItemView, MapItemViewModel>, IMapItemViewFactory
    {
    public override string AssetName => "UI/CheatWindow/MapItem";
    }
}