using UnityEngine;

namespace vikwhite
{
    public static class GameObjectExtensions
    {
        public static GameObject ResetChildrenTransforms(this GameObject go)
        {
            foreach (Transform child in go.transform) {
                child.position = Vector3.zero;
                child.rotation = Quaternion.identity;
            }
            return go;
        }
    }
}