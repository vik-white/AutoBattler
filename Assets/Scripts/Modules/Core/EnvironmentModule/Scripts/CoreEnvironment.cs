using UnityEngine;

namespace vikwhite
{
    public class CoreEnvironment : Environment
    {
        protected override void Register()
        {
            Register<AssetLoaderModuleDependency>();
            Register<EntityModuleDependency>();
            Register<EventModuleDependency>();
            Register<MvvmModuleDependency>();
            Register<WindowModuleDependency>();
            Register<EnvironmentModuleDependency>();
        }

        protected override void Initialize()
        {
            Resolve<IWindowViewFactory>().Initialize(GameObject.FindAnyObjectByType<Canvas>().transform);
        }
    }
}