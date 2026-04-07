using UnityEngine;

namespace vikwhite
{
    public class ResourceAssetLoader : IAssetLoader
    {
        public GameObject Load(string path)
        {
            return Resources.Load<GameObject>(path);
        }
    }
}