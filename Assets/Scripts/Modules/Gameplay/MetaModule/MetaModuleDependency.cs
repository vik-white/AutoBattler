namespace vikwhite
{
    public class MetaModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IMetaWindow, MetaWindow>();
            Register<MetaWindowViewModel>();
            Register<MetaWindowView>();
            
            Register<IMetaItemViewFactory, MetaItemViewFactory>();
            Register<MetaItemViewModel>();
            Register<MetaItemView>();
            Register<MetaStarsViewModel>();
            Register<MetaStarsView>();
        }
    }
}
