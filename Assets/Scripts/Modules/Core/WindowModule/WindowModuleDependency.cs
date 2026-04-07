namespace vikwhite
{
    public class WindowModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IWindowViewFactory, WindowViewFactory>();
            Register<IWindowManager, WindowManager>();
        }
    }
}