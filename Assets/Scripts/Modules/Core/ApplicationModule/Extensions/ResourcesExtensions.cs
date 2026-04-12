using UnityEngine;

namespace vikwhite
{
    public static class ResourcesExtensions
    {
        public static T Load<T>(string[] paths, string name) where T : UnityEngine.Object
        {
            foreach (var path in paths)
            {
                T result = Resources.Load<T>(path + name);
                if (result != null) return result;
            }

            return null;
        }
    }
}