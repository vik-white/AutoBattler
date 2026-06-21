namespace vikwhite
{
    public interface IMetaItemViewFactory : IPooledViewFactory<MetaItemView, MetaItemViewModel> { }
    
    public class MetaItemViewFactory : PooledViewFactory<MetaItemView, MetaItemViewModel>, IMetaItemViewFactory
    {
        public override string AssetName => "UI/Prefabs/Elements/MetaItem";
    }
}
