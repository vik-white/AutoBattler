namespace vikwhite
{
    public class MvvmModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IViewFactory, ViewFactory>();
            Register<IViewModelFactory, ViewModelFactory>();
        }
    }
}