using System.Collections;
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
            Register<LocationModuleDependency>();
        }

        protected override IEnumerator Initialize()
        {
            Resolve<IWindowViewFactory>().Initialize(GameObject.FindAnyObjectByType<Canvas>().transform);
            yield return null;
        }
    }
}