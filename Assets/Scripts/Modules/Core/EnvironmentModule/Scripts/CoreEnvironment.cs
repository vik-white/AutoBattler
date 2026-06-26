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
            Register<CameraModuleDependency>();
            Register<MvvmModuleDependency>();
            Register<WindowModuleDependency>();
            Register<EnvironmentModuleDependency>();
            Register<LocationModuleDependency>();
        }

        protected override IEnumerator Initialize()
        {
            var canvas = FindRootCanvas();
            Resolve<IUIRoot>().Initialize(canvas.GetComponent<RectTransform>());
            yield return null;
        }

        private static Canvas FindRootCanvas()
        {
#if UNITY_2023_1_OR_NEWER
            var canvases = GameObject.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude);
#else
            var canvases = GameObject.FindObjectsOfType<Canvas>();
#endif
            foreach (var canvas in canvases)
            {
                if (canvas.name != "Overlay Canvas")
                    return canvas;
            }

            return GameObject.FindAnyObjectByType<Canvas>();
        }
    }
}
