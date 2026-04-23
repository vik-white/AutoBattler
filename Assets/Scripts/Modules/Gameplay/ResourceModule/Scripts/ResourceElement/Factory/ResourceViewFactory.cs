namespace vikwhite
{
    public interface IResourceViewFactory : IPooledViewFactory<ResourceView, ResourceViewModel> { }
    
    public class ResourceViewFactory : PooledViewFactory<ResourceView, ResourceViewModel>, IResourceViewFactory
    {
        public override string AssetName => "UI/Prefabs/Elements/Resource";
    }
}