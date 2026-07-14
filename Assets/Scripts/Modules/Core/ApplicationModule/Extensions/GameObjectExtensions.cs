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
        
        public static void ClearChildren(this GameObject go)
        {
            for (int i = go.transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(go.transform.GetChild(i).gameObject);
            }
        }
    }
}