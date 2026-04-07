namespace vikwhite
{
    public class AssetLoaderModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IAssetLoader, ResourceAssetLoader>();
        }
    }
}