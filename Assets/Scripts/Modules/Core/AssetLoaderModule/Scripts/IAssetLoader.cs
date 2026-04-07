using UnityEngine;

namespace vikwhite
{
    public interface IAssetLoader
    {
        GameObject Load(string path);
    }
}