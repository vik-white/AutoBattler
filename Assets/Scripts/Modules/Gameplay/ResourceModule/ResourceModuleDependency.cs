namespace vikwhite
{
    public class ResourceModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IResourceService, ResourceService>();
            
            Register<IResourceViewFactory, ResourceViewFactory>();
            Register<ResourceViewModel>();
            Register<ResourceView>();
        }
    }
}